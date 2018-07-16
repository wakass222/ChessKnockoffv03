using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using ChessKnockoff.Models;
using ChessDotNet;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ChessKnockoff
{
    public class GameHub : Hub
    {
        /// <summary>
        /// Stops the search for a game
        /// </summary>
        /// <returns>A Task to track the asynchronous method execution.</returns>
        public void QuitFindGame()
        {
            //Create a player connection object to store related connection data
            playerConnection quittingPlayer = new playerConnection(Context.User.Identity, this.Context.ConnectionId);
            GameState.Instance.RemoveFromWaitingPool(quittingPlayer);

        }

        /// <summary>
        /// Matches the player with another opponent
        /// </summary>
        /// <returns>A Task to track the asynchronous method execution.</returns>
        public async Task FindGame()
        {
            //Create a player connection object to store related connection data
            playerConnection joiningPlayer = new playerConnection(Context.User.Identity, this.Context.ConnectionId);

            // Find any pending games if any
            playerConnection opponent = GameState.Instance.GetWaitingOpponent();
            if (opponent == null)
            {
                // No waiting players so enter the waiting pool
                GameState.Instance.AddToWaitingPool(joiningPlayer);
                this.Clients.Caller.waitingList();
            }
            else
            {
                //Create a new random object
                Random rand = new Random();
                bool randomBool = rand.Next(0, 2) == 0;

                // An opponent was found so make a new game
                Game newGame = await GameState.Instance.CreateGame(opponent, joiningPlayer);

                //Make sure to HTML encode both players' username
                string opponentUsername = HttpUtility.HtmlEncode(opponent.userInformation.UserName);
                string joiningPlayerUsername = HttpUtility.HtmlEncode(joiningPlayer.userInformation.UserName);

                //Hold the fen string for the new game
                string fenString = newGame.Board.GetFen();

                //Randomly assign the side the player is playing on
                if (randomBool)
                {
                    //Set the respective players side
                    joiningPlayer.side = Player.Black;
                    opponent.side = Player.White;

                    //The joining client
                    Clients.Client(this.Context.ConnectionId).start(fenString, opponentUsername, "black");
                    //The opponent client
                    Clients.Client(opponent.connectionString).start(fenString, joiningPlayerUsername, "white");
                }
                else
                {
                    //Set the respective players side
                    joiningPlayer.side = Player.White;
                    opponent.side = Player.Black;

                    //The joining client
                    Clients.Client(this.Context.ConnectionId).start(fenString, opponentUsername, "white");
                    //The opponent client
                    Clients.Client(opponent.connectionString).start(fenString, joiningPlayerUsername, "black");
                }
            }
        }

        /// <summary>
        /// Client has requested to place a piece down in the following position.
        /// </summary>
        /// <param name="row">The row part of the position.</param>
        /// <param name="col">The column part of the position.</param>
        /// <returns>A Task to track the asynchronous method execution.<</returns>
        public void MakeTurn(string sourcePosition, string destinationPosition)
        {
            playerConnection playerMakingTurn = GameState.Instance.GetPlayer(this.Context.ConnectionId);
            playerConnection opponent;
            Game game;

            game = GameState.Instance.GetGame(playerMakingTurn, out opponent);

            //Create the move object
            Move move = new Move(sourcePosition, destinationPosition, playerMakingTurn.side);

            //Check if the move valid, if is not valid then do nothing since there is client side validation
            if (game.Board.IsValidMove(move))
            {
                //Apply that move
                game.Board.ApplyMove(move, true);
            }

            //Else just return the current board state
            this.Clients.Group(game.Id).UpdatePosition(game.Board.GetFen());
        }

        /// <summary>
        /// A player that is leaving should end all games and notify the opponent.
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override async Task OnDisconnected(bool stopCalled)
        {
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
                    //Remove them from the collection of games
                    GameState.Instance.RemoveGame(ongoingGame.Id);
                }
            }

            await base.OnDisconnected(stopCalled);
        }
    }
}