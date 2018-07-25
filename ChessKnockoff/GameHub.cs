using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using ChessKnockoff.Models;
using ChessDotNet;

namespace ChessKnockoff
{
    /// <summary>
    /// Class that inherits from hub and provides server functions the client can call
    /// </summary>
    public class GameHub : Hub
    {
        /// <summary>
        /// Converts the enum value to a string
        /// </summary>
        /// <param name="player">The Player side enum</param>
        /// <returns></returns>
        private string playerEnumToString(Player player)
        {
            //If they are black
            if (player == Player.Black)
            {
                //Return black
                return "black";
            } else
            {
                //Else return white if they are not black
                return "white";
            }
        }

        /// <summary>
        /// Stops the search for a game
        /// </summary>
        /// <returns>A Task to track the asynchronous method execution.</returns>
        public void QuitFindGame()
        {
            //Create a player connection object to store related connection data
            playerConnection quittingPlayer = GameState.Instance.GetPlayer(this.Context.ConnectionId);
            GameState.Instance.RemoveFromWaitingPool(quittingPlayer);

        }

        /// <summary>
        /// Matches the player with another opponent
        /// </summary>
        /// <returns>A Task to track the asynchronous method execution.</returns>
        public async Task FindGame()
        {
            //Create a player connection object to store related connection data
            playerConnection joiningPlayer = new playerConnection(HttpContext.Current.Session["Username"].ToString(), this.Context.ConnectionId);

            //Check if the player is already waiting or in game from another client to avoid pairing
            if (GameState.Instance.playerAlreadyExists(joiningPlayer.Username))
            {
                //Inform the client that they are already playing
                Clients.Caller.AlreadyPlaying();
            }             
            else if (!GameState.Instance.isThereWaitingOpponent()) //Check if the player is already waiting
            {
                // No waiting players so enter the waiting pool
                GameState.Instance.AddToWaitingPool(joiningPlayer);
                Clients.Caller.InQueue();
            }
            else
            {
                // Find any pending games if any
                playerConnection opponent = GameState.Instance.GetWaitingOpponent();

                //Create a new random object
                Random rand = new Random();
                bool randomBool = rand.Next(0, 2) == 0;

                // An opponent was found so make a new game
                Game newGame = await GameState.Instance.CreateGame(opponent, joiningPlayer);

                //Make sure to HTML encode both players' username
                string opponentUsername = HttpUtility.HtmlEncode(opponent.Username);
                string joiningPlayerUsername = HttpUtility.HtmlEncode(joiningPlayer.Username);

                //Hold the fen string for the new game
                string fenString = newGame.Board.GetFen();

                //Randomly assign the side the player is playing on
                if (randomBool)
                {
                    //Set the respective players side
                    joiningPlayer.side = Player.Black;
                    opponent.side = Player.White;

                    //The joining client
                    Clients.Client(this.Context.ConnectionId).Start(fenString, opponentUsername, "black");
                    //The opponent client
                    Clients.Client(opponent.connectionString).Start(fenString, joiningPlayerUsername, "white");
                }
                else
                {
                    //Set the respective players side
                    joiningPlayer.side = Player.White;
                    opponent.side = Player.Black;

                    //The joining client
                    Clients.Client(this.Context.ConnectionId).Start(fenString, opponentUsername, "white");
                    //The opponent client
                    Clients.Client(opponent.connectionString).Start(fenString, joiningPlayerUsername, "black");
                }
            }

        }

        /// <summary>
        /// Client has requested to place a piece down in the following position.
        /// </summary>
        /// <param name="sourcePosition">The original position of the piece.</param>
        /// <param name="destinationPosition">The destination of the piece.</param>
        /// <returns>A Task to track the asynchronous method execution.<</returns>
        public void MakeTurn(string sourcePosition, string destinationPosition)
        {
            playerConnection playerMakingTurn = GameState.Instance.GetPlayer(this.Context.ConnectionId);
            playerConnection opponent;
            Game game;

            game = GameState.Instance.GetGame(playerMakingTurn, out opponent);

            //Check if the board state changed
            string fenTemp = game.Board.GetFen();

            //Try to apply the move
            try
            {
                //Create the move object and always promote to queen if possible
                Move move = new Move(sourcePosition, destinationPosition, playerMakingTurn.side, 'Q');

                //Apply that move and pass the to check parameter
                game.Board.ApplyMove(move, false);

                if (fenTemp != game.Board.GetFen())
                {
                    //If an exception does not occur then reset the timer
                    game.timerWarning.Stop();
                    game.timerWarning.Start();

                    //Also stop the final warning timer
                    game.timerFinal.Stop();
                }
            } catch(Exception)
            {
                //Do nothing with the exception
            }

            //Always return the position even if it is not valid so that the piece returns to its original position on the client
            //The method is void therefore to a call needs to be made to parse the board state
            this.Clients.Group(game.Id).UpdatePosition(game.Board.GetFen(), playerEnumToString(game.Board.WhoseTurn));
            
            //If either player has stalemated
            if (game.Board.IsStalemated(playerMakingTurn.side))
            {
                //Update both players' ELO with the stalemate calculation
                GameState.Instance.updateELO(playerMakingTurn.Username, opponent.Username, 0.5);

                //Notify both clients that a draw has occured
                this.Clients.Group(game.Id).gameDraw();

                // Remove the game (in any game over scenario) to reclaim resources
                GameState.Instance.RemoveGame(game.Id);
            }
            else if (game.Board.IsCheckmated(opponent.side)) //Check if there is a winner
            {
                //Update both players' ELO, in which the player making the turn won
                GameState.Instance.updateELO(playerMakingTurn.Username, opponent.Username, 1);

                //Notify both clients that the player making the turn has won
                this.Clients.Group(game.Id).gameFinish(playerEnumToString(playerMakingTurn.side));

                // Remove the game (in any game over scenario) to reclaim resources
                GameState.Instance.RemoveGame(game.Id);
            }
        }

        /// <summary>
        /// A player that is leaving should end all games and notify the opponent.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
            //Players are only added to the player list once they are in a game, therefore it is only necessary to remove the game along with its players
            playerConnection leavingPlayer = GameState.Instance.GetPlayer(playerId: this.Context.ConnectionId);

            // Only handle cases where user was a player in a game or waiting for an opponent
            if (leavingPlayer != null)
            {
                //Get the game of the leaving player
                playerConnection opponent;
                Game ongoingGame = GameState.Instance.GetGame(leavingPlayer, out opponent);
                
                //If there was a game
                if (ongoingGame != null)
                {
                    //Display that the opponent left to the client
                    this.Clients.Group(ongoingGame.Id).opponentLeft();

                    //Remove the afk timers since they are not needed
                    ongoingGame.timerWarning.Dispose();
                    ongoingGame.timerFinal.Dispose();

                    //Also count as a loss for the opponent
                    GameState.Instance.updateELO(leavingPlayer.Username, opponent.Username, 0);

                    //Remove them from the collection of games
                    GameState.Instance.RemoveGame(ongoingGame.Id);
                }
            }

            await base.OnDisconnected(stopCalled);
        }
    }
}