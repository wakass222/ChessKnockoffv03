using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessDotNet;
using Microsoft.AspNet.SignalR;
using ChessDotNet.Variants.Horde;
using System.Timers;

namespace ChessKnockoff.Models
{
    public class Game
    {
        /// <summary>
        /// Creates a new game object.
        /// </summary>
        /// <param name="firstPlayer">The first player to join the game.</param>
        /// <param name="secondPlayer">The second player to join the game.</param>
        public Game(playerConnection firstPlayer, playerConnection secondPlayer)
        {
            this.firstPlayer = firstPlayer;
            this.secondPlayer = secondPlayer;
            this.Id = Guid.NewGuid().ToString("d");
            this.Board = new HordeChessGame();

            // Link the players to the game as well
            this.firstPlayer.GameId = this.Id;
            this.secondPlayer.GameId = this.Id;

            //Start the timer as well
            this.timer = new Timer(turnTime);
            //Attach the lambda to the event to call the player is afk function
            timer.Elapsed += (sender, e) => PlayerIsAfk();
            //Make sure that it only executes once
            timer.AutoReset = false;
            //Start the timer
            timer.Start();
        }

        public Timer timer;

        //Only allow this property to be read
        //Sets how long each turn can be
        public static int turnTime { get; } = 30000;

        /// <summary>
        /// Calls if the player is afk and does all the required actions
        /// </summary>
        /// <param name="gameId">The unique identifier of the game.</param>
        private void PlayerIsAfk()
        {
            //Gets the GameHub so client functions can be invoked here
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

            //Check which player is AFK
            if (this.Board.WhoseTurn == this.firstPlayer.side)
            {
                //The first player is afk therefore they lose
                GameState.Instance.updateELO(this.firstPlayer.Username, this.secondPlayer.Username, 0);
                //In form both clients of what happened
                context.Clients.Client(this.firstPlayer.connectionString).afkWin();
                context.Clients.Client(this.secondPlayer.connectionString).afkLose();
            } else
            {
                //The second payer is afk therefore the first player wins
                GameState.Instance.updateELO(this.firstPlayer.Username, this.secondPlayer.Username, 1);
                //In form both clients of what happened
                context.Clients.Client(this.firstPlayer.connectionString).afkLose();
                context.Clients.Client(this.secondPlayer.connectionString).afkWin();
            }

            //Get rid of the timer since it is not needed anymore
            this.timer.Dispose();

            //Remove the game from the list of all current games
            GameState.Instance.RemoveGame(this.Id);
        }

        /// <summary>
        /// A unique identifier for the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The first player
        /// </summary>
        public playerConnection firstPlayer { get; set; }

        /// <summary>
        /// The second player
        /// </summary>
        public playerConnection secondPlayer { get; set; }

        /// <summary>
        /// Create a ChessGame object to store the chess game
        /// </summary>
        public ChessGame Board { get; set; }
    }
}
