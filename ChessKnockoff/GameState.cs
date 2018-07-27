using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessKnockoff.Models;
using ChessDotNet;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.SqlClient;

namespace ChessKnockoff
{
    /// <summary>
    /// Statically stores all games and players
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Holds the singleton instance
        /// Also removes the need for locks or checks in order to be thread safe
        /// </summary>
        private readonly static Lazy<GameState> instance =
            new Lazy<GameState>(() => new GameState(GlobalHost.ConnectionManager.GetHubContext<GameHub>()));

        /// <summary>
        /// Holds all the players. Key is their connection string
        /// </summary>
        private readonly ConcurrentDictionary<string, playerConnection> players =
            new ConcurrentDictionary<string, playerConnection>();

        /// <summary>
        /// Holds all the games. Key is the game id
        /// </summary>
        private readonly ConcurrentDictionary<string, Game> games =
            new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A dictionary of players that are waiting for an opponent
        /// </summary>
        private readonly ConcurrentDictionary<string, playerConnection> waitingPlayers =
            new ConcurrentDictionary<string, playerConnection>();

        /// <summary>
        /// Private constructor used by the Lazy initialisation of this class
        /// </summary>
        /// <param name="context">The signalir hub context</param>
        private GameState(IHubContext context)
        {
            this.Clients = context.Clients;
            this.Groups = context.Groups;
        }

        /// <summary>
        /// A public property to access the class' values and information
        /// </summary>
        public static GameState Instance
        {
            get { return instance.Value; }
        }

        public IHubConnectionContext<dynamic> Clients { get; set; }

        public IGroupManager Groups { get; set; }

        /// <summary>
        /// Retrieves the player that has the given ID.
        /// </summary>
        /// <param name="playerId">The unique identifier of the player to find.</param>
        /// <returns>The found player; otherwise null.</returns>
        public playerConnection GetPlayerInGame(string playerId)
        {
            playerConnection foundPlayer;
            if (!players.TryGetValue(playerId, out foundPlayer))
            {
                return null;
            }

            return foundPlayer;
        }

        /// <summary>
        /// Retrieves the game that the given player is playing in.
        /// </summary>
        /// <param name="playerId">The player in the game.</param>
        /// <param name="opponent">The opponent of the player if there is one; otherwise null.</param>
        /// <returns>The game that the specified player is a member of if game is found; otherwise null.</returns>
        public Game GetGame(playerConnection player, out playerConnection opponent)
        {
            opponent = null;
            Game foundGame = games.Values.FirstOrDefault(g => g.Id == player.GameId);

            if (foundGame == null)
            {
                return null;
            }

            opponent = (player == foundGame.firstPlayer) ?
                foundGame.secondPlayer :
                foundGame.firstPlayer;

            return foundGame;
        }

        /// <summary>
        /// Determines if the player is already waiting or in a game
        /// </summary>
        /// <param name="playerToCheck">The player to check.</param>
        /// <returns>true if the player is in a game or waiting</returns>
        public bool playerAlreadyExists(string Username)
        {
            //Check if player is in game
            bool playerInGame = this.players.Values.FirstOrDefault(player => player.Username == Username) != null;

            //Check if they are in queue
            bool playerIsWaiting = this.waitingPlayers.Values.FirstOrDefault(player => player.Username == Username) != null;

            //If the player is waiting or in queue then return true
            if (playerInGame || playerIsWaiting)
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates both players ELO
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo">The actual result, 1 for win; 0 for loss; 0.5 for draw</param>
        /// <param name="resultOfPlayerOne"></param>
        public void updateELO(string playerOneUsername, string playerTwoUsername, double resultOfPlayerOne)
        {
            //Finds the player and their ELO
            string queryString = "SELECT ELO FROM Player WHERE Username=@Username";

            //Create the database connection and command then dispose when done
            using (SqlConnection connectionSelect = new SqlConnection(ExtendedPage.dbConnectionString))
            using (SqlCommand commandSelect = new SqlCommand(queryString, connectionSelect))
            {
                //Open the database connection
                connectionSelect.Open();

                //Add the parameters
                commandSelect.Parameters.AddWithValue("@Username", playerOneUsername);

                //Store the player one ELO
                int playerOneElo = (int)commandSelect.ExecuteScalar();

                //Clear the parameters
                commandSelect.Parameters.Clear();

                //Add the parameter for the second user
                commandSelect.Parameters.AddWithValue("@Username", playerTwoUsername);

                //Store the player two ELO
                int playerTwoElo = (int)commandSelect.ExecuteScalar();

                //Calculate the different between them
                double eloDifference = playerTwoElo - playerOneElo;

                //Calculate the odds of player one would win
                //A cast has to be used so there is enough precision
                double expectationOfPlayerOne = 1 / (1 + Math.Pow(10, eloDifference / 400));

                //Calculate how much to update the ELO by
                //K-factor of 20 is used
                int updateValue = Convert.ToInt32((double)20 * ((double)resultOfPlayerOne - expectationOfPlayerOne));

                //ELO is a zero sum game, the amount lost is equivalent to the amount gained
                playerOneElo += updateValue;
                playerTwoElo -= updateValue;

                //Save the results into the database
                queryString = "UPDATE Player SET ELO = @ELO WHERE Username=@Username";

                //Create the database connection and command then dispose when done
                using (SqlConnection connectionUpdate = new SqlConnection(ExtendedPage.dbConnectionString))
                using (SqlCommand commandUpdate = new SqlCommand(queryString, connectionUpdate))
                {
                    //Open the database connection
                    connectionUpdate.Open();

                    //Update player one's elo
                    commandUpdate.Parameters.AddWithValue("@Username", playerOneUsername);
                    commandUpdate.Parameters.AddWithValue("@ELO", playerOneElo);
                    commandUpdate.ExecuteNonQuery();

                    //Then clear the parameters
                    commandUpdate.Parameters.Clear();

                    //Now update player two's ELO
                    commandUpdate.Parameters.AddWithValue("@Username", playerTwoUsername);
                    commandUpdate.Parameters.AddWithValue("@ELO", playerTwoElo);
                    commandUpdate.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Check if there is waiting opponents
        /// </summary>
        /// <returns>Returns true if there are opponents waiting else false</returns>
        public bool isThereWaitingOpponent()
        {
            if (waitingPlayers.IsEmpty)
            {
                return false;
            } else
            {
                return true;
            }
        }

        /// <summary>
        /// Retrieves a game waiting for players.
        /// </summary>
        /// <returns>Returns a pending game if any; otherwise returns null.</returns>
        public playerConnection GetWaitingOpponent()
        {
            playerConnection foundPlayer;
            waitingPlayers.TryRemove(this.waitingPlayers.Keys.FirstOrDefault(), out foundPlayer);

            return foundPlayer;
        }

        /// <summary>
        /// Forgets the specified game. Use if the game is over.
        /// No need to manually remove a user from a group when the connection ends.
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        /// <returns>A task to track the asynchronous method execution.</returns>
        public void RemoveGame(string gameId)
        {
            // Remove the game
            Game foundGame;
            if (!games.TryRemove(gameId, out foundGame))
            {
                throw new InvalidOperationException("Game not found.");
            }

            //Stop the afk timers to prevent execution after the game ahs finished
            foundGame.timerWarning.Stop();
            foundGame.timerFinal.Stop();

            //Remove the afk timers since they are not needed
            foundGame.timerWarning.Dispose();
            foundGame.timerFinal.Dispose();

            // Remove the players
            playerConnection foundPlayer;
            players.TryRemove(foundGame.firstPlayer.connectionString, out foundPlayer);
            players.TryRemove(foundGame.secondPlayer.connectionString, out foundPlayer);
        }

        /// <summary>
        /// Removes specified player from the waiting pool.
        /// </summary>
        /// <param name="player">The player to remove from the waiting pool.</param>
        public void RemoveFromWaitingPool(string connectionString)
        {
            //Create throwaway since method requires an out argument
            playerConnection throwAway;
            waitingPlayers.TryRemove(connectionString, out throwAway);
        }

        /// <summary>
        /// Adds specified player to the waiting pool.
        /// </summary>
        /// <param name="player">The player to add to waiting pool.</param>
        public void AddToWaitingPool(playerConnection player)
        {
            waitingPlayers.TryAdd(player.connectionString, player);
        }

        /// <summary>
        /// Creates a new pending game which will be waiting for more players.
        /// </summary>
        /// <param name="joiningPlayer">The first player to enter the game.</param>
        /// <returns>The newly created game in a pending state.</returns>
        public async Task<Game> CreateGame(playerConnection firstPlayer, playerConnection secondPlayer)
        {
            // Define the new game and add to waiting pool
            Game game = new Game(firstPlayer, secondPlayer);
            this.games[game.Id] = game;

            //Store the users in a dictionary
            this.players[firstPlayer.connectionString] = firstPlayer;
            this.players[secondPlayer.connectionString] = secondPlayer;

            // Create a new group to manage communication using ID as group name
            await this.Groups.Add(firstPlayer.connectionString, groupName: game.Id);
            await this.Groups.Add(secondPlayer.connectionString, groupName: game.Id);

            return game;
        }
    }
}