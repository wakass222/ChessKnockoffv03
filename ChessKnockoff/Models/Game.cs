using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessDotNet;
using Microsoft.AspNet.SignalR;
using ChessDotNet.Variants.Horde;

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
            this.Board = new ChessGame();

            // Link the players to the game as well
            this.firstPlayer.GameId = this.Id;
            this.secondPlayer.GameId = this.Id;
        }

        /// <summary>
        /// A unique identifier for the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The black player
        /// </summary>
        public playerConnection firstPlayer { get; set; }

        /// <summary>
        /// The white player
        /// </summary>
        public playerConnection secondPlayer { get; set; }

        /// <summary>
        /// Create a ChessGame object to store the chess game
        /// </summary>
        public ChessGame Board { get; set; }
    }
}
