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

namespace ChessKnockoff
{
    /// <summary>
    /// This class can statically persist a collection of players and
    /// matches that each of the players are playing using the singleton pattern.
    /// The singleton pattern restricts the instantiation of the class to one object.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// Singleton instance that defers initialization until access time.
        /// </summary>
        private readonly static Lazy<GameState> instance =
            new Lazy<GameState>(() => new GameState(GlobalHost.ConnectionManager.GetHubContext<GameHub>()));

        /// <summary>
        /// A unique identifier for this game. Also used as the group name.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// One of two partipants of the game.
        /// </summary>
        public playerConnection playerWhite { get; set; }

        /// <summary>
        /// One of two participants of the game.
        /// </summary>
        public playerConnection playerBlack { get; set; }

        /// <summary>
        /// The board that represents the tic-tac-toe game.
        /// </summary>
        public ChessGame Board { get; set; }

        /// <summary>
        /// Creates a new game object.
        /// </summary>
        /// <param name="player1">The first player to join the game.</param>
        /// <param name="player2">The second player to join the game.</param>
        public GameState(playerConnection playerWhite, playerConnection playerBlack)
        {
            this.playerWhite = playerWhite;
            this.playerBlack = playerBlack;
            this.Id = Guid.NewGuid().ToString("d");
            this.Board = new ChessGame();

            // Link the players to the game as well
            this.playerWhite.GameId = this.Id;
            this.playerBlack.GameId = this.Id;
        }

        /// <summary>
        /// A reference to all players. Key is the unique ID of the player.
        /// Note that this collection is concurrent to handle multiple threads.
        /// </summary>
        private readonly ConcurrentDictionary<string, playerConnection> players =
            new ConcurrentDictionary<string, playerConnection>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A reference to all games. Key is the group name of the game.
        /// Note that this collection uses a concurrent dictionary to handle multiple threads.
        /// </summary>
        private readonly ConcurrentDictionary<string, Game> games =
            new ConcurrentDictionary<string, Game>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// A queue of players that are waiting for an opponent.
        /// </summary>
        private readonly ConcurrentQueue<playerConnection> waitingPlayers =
            new ConcurrentQueue<playerConnection>();

        private GameState(IHubContext context)
        {
            this.Clients = context.Clients;
            this.Groups = context.Groups;
        }

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
        public playerConnection GetPlayer(string playerId)
        {
            playerConnection foundPlayer;
            if (!this.players.TryGetValue(playerId, out foundPlayer))
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
            Game foundGame = this.games.Values.FirstOrDefault(g => g.Id == player.GameId);

            if (foundGame == null)
            {
                return null;
            }

            opponent = (player == foundGame.playerWhite) ?
                foundGame.playerBlack :
                foundGame.playerWhite;

            return foundGame;
        }

        /// <summary>
        /// Retrieves a game waiting for players.
        /// </summary>
        /// <returns>Returns a pending game if any; otherwise returns null.</returns>
        public playerConnection GetWaitingOpponent()
        {
            playerConnection foundPlayer;
            if (!this.waitingPlayers.TryDequeue(out foundPlayer))
            {
                return null;
            }

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
            if (!this.games.TryRemove(gameId, out foundGame))
            {
                throw new InvalidOperationException("Game not found.");
            }

            // Remove the players, best effort
            playerConnection foundPlayer;
            this.players.TryRemove(foundGame.playerWhite.connectionString, out foundPlayer);
            this.players.TryRemove(foundGame.playerBlack.connectionString, out foundPlayer);
        }

        /// <summary>
        /// Adds specified player to the waiting pool.
        /// </summary>
        /// <param name="player">The player to add to waiting pool.</param>
        public void AddToWaitingPool(playerConnection player)
        {
            this.waitingPlayers.Enqueue(player);
        }

        /// <summary>
        /// Creates a new pending game which will be waiting for more players.
        /// </summary>
        /// <param name="joiningPlayer">The first player to enter the game.</param>
        /// <returns>The newly created game in a pending state.</returns>
        public async Task<Game> CreateGame(playerConnection whitePlayer, playerConnection blackPlayer)
        {
            // Define the new game and add to waiting pool
            Game game = new Game(whitePlayer, blackPlayer);
            this.games[game.Id] = game;
            
            // Create a new group to manage communication using ID as group name
            await this.Groups.Add(whitePlayer.connectionString, groupName: game.Id);
            await this.Groups.Add(blackPlayer.connectionString, groupName: game.Id);

            return game;
        }
    }
}