using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessDotNet;
using ChessDotNet.Variants.Horde;
using Microsoft.AspNet.SignalR;

namespace ChessKnockoff.Models
{
    public class Game
    {
        /// <summary>
        /// Creates a new game object.
        /// </summary>
        /// <param name="playerWhite">The first player to join the game.</param>
        /// <param name="playerBlack">The second player to join the game.</param>
        public Game(playerConnection playerWhite, playerConnection playerBlack)
        {
            //Set the object properties
            this.playerWhite = playerWhite;
            this.playerBlack = playerBlack;
            this.Id = Guid.NewGuid().ToString("d");
            this.Board = new HordeChessGame();
            this.fenString = Board.GetFen();

            // Link the players to the game as well
            this.playerWhite.GameId = this.Id;
            this.playerBlack.GameId = this.Id;

        }

        /// <summary>
        /// A unique identifier for the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The black player
        /// </summary>
        public playerConnection playerBlack { get; set; }

        /// <summary>
        /// The white player
        /// </summary>
        public playerConnection playerWhite { get; set; }

        /// <summary>
        /// Create a ChessGame object to store the chess game
        /// </summary>
        public ChessGame Board { get; set; }

        /// <summary>
        /// Holds the board state
        /// </summary>
        public string fenString { get; set; }
    }
}
