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
    public class GameHub : Hub
    {
        /// <summary>
        /// Matches the player with another opponent
        /// </summary>
        /// <param name="username">The friendly name that the user has chosen.</param>
        /// <returns>A Task to track the asynchronous method execution.</returns>
        public async Task FindGame(playerConnection joiningPlayer)
        {
            this.Clients.Caller.playerJoined(joiningPlayer);

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
                // An opponent was found so join a new game and start the game
                // Opponent is first player since they were waiting first
                Game newGame = await GameState.Instance.CreateGame(opponent, joiningPlayer);
                Clients.Group(newGame.Id).start(newGame);
            }
        }

        /// <summary>
        /// Client has requested to place a piece down in the following position.
        /// </summary>
        /// <param name="row">The row part of the position.</param>
        /// <param name="col">The column part of the position.</param>
        /// <returns>A Task to track the asynchronous method execution.<</returns>
        public void makeTurn(string sourcePosition, string destinationPosition)
        {
            playerConnection playerMakingTurn = GameState.Instance.GetPlayer(playerId: this.Context.ConnectionId);
            playerConnection opponent;
            Game game = GameState.Instance.GetGame(playerMakingTurn, out opponent);

            //Create the move object
            Move move = new Move(sourcePosition, destinationPosition, playerMakingTurn.side);

            /*
            if (game.Board.IsCheckmated(opponent.side))
            {
                this.Clients.Client(playerMakingTurn.applicationUser)
            }
            */
            //Check if the move valid, if is not valid then do nothing since there is client side validation
            if (game.Board.IsValidMove(move))
            {
                game.Board.ApplyMove(move, true);
                this.Clients.Group(game.Id).updatePosition(game.Board.GetFen());
            }
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
                playerConnection opponent;
                Game ongoingGame = GameState.Instance.GetGame(leavingPlayer, out opponent);
                if (ongoingGame != null)
                {
                    this.Clients.Group(ongoingGame.Id).opponentLeft();
                    GameState.Instance.RemoveGame(ongoingGame.Id);
                }
            }

            await base.OnDisconnected(stopCalled);
        }
    }
}