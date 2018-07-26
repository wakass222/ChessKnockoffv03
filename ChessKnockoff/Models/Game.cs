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
            //Store the players
            this.firstPlayer = firstPlayer;
            this.secondPlayer = secondPlayer;

            //Create a unique identifier for the game
            this.Id = Guid.NewGuid().ToString("d");
            this.Board = new HordeChessGame();

            // Link the players to the game as well
            this.firstPlayer.GameId = this.Id;
            this.secondPlayer.GameId = this.Id;

            //Start the timer as well
            this.timerWarning = new Timer(warningTime);
            //Attach the lambda to the event to call the player is afk function
            timerWarning.Elapsed += (sender, e) => PlayerIsAfkWarning();
            //Make sure that it only executes once
            timerWarning.AutoReset = false;
            //Start the timer
            timerWarning.Start();

            //Create the warning timer as well but do not start it
            this.timerFinal = new Timer(finalTime);
            //Attach the lambda to the event to call the player is afk function
            timerFinal.Elapsed += (sender, e) => PlayerIsAfk();
            //Make sure that it only executes once
            timerFinal.AutoReset = false;
        }


        /// <summary>
        /// A timer that when activated causes a warning
        /// </summary>
        public Timer timerWarning;
        /// <summary>
        /// A timer that ends the game causing an automatic forfeit
        /// </summary>
        public Timer timerFinal;

        /// <summary>
        /// Sets how long before the warning appears, 30 seconds
        /// </summary>
        public static int warningTime { get; } = 30000;

        /// <summary>
        /// Sets the time after the warning that results in a loss if still afk, 10 seconds
        /// </summary>
        public static int finalTime { get; } = 10000;

        /// <summary>
        /// Call to notify the player to make a move and starts the final timer
        /// </summary>
        private void PlayerIsAfkWarning()
        {
            //Gets the GameHub so client functions can be invoked here
            var context = GlobalHost.ConnectionManager.GetHubContext<GameHub>();

            //Notify the user to make a move 
            if (this.Board.WhoseTurn == this.firstPlayer.side)
            {
                //Notify the first player to make a move
                context.Clients.Client(firstPlayer.connectionString).AfkWarning();
            } else
            {
                //NOtify the second player to make a move
                context.Clients.Client(secondPlayer.connectionString).AfkWarning();
            }

            //Start the timer for the final action
            this.timerFinal.Start();
        }

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
                context.Clients.Client(this.firstPlayer.connectionString).afkLose();
                context.Clients.Client(this.secondPlayer.connectionString).afkWin();
            } else
            {
                //The second payer is afk therefore the first player wins
                GameState.Instance.updateELO(this.firstPlayer.Username, this.secondPlayer.Username, 1);
                //In form both clients of what happened
                context.Clients.Client(this.firstPlayer.connectionString).afkWin();
                context.Clients.Client(this.secondPlayer.connectionString).afkLose();
            }

            //Stop any ongoing timers
            this.timerWarning.Stop();
            this.timerFinal.Stop();

            //Get rid of the timers since it is not needed anymore
            this.timerFinal.Dispose();
            this.timerWarning.Dispose();

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
