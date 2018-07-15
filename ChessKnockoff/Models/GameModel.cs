using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessDotNet;
using Microsoft.AspNet.SignalR;

namespace ChessKnockoff.Models
{
    public class Game
    {
        /// <summary>
        /// A unique identifier for the game.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The black player
        /// </summary>
        public ApplicationUser playerBlack { get; set; }

        /// <summary>
        /// The white player
        /// </summary>
        public ApplicationUser playerWhite { get; set; }

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
