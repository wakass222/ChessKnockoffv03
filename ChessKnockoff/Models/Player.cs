using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChessDotNet;

namespace ChessKnockoff.Models
{
    public class playerConnection
    {
        /// <summary>
        /// Which side the player is: black or white
        /// </summary>
        public Player side { get; set; }

        /// <summary>
        /// The Identity model
        /// </summary>
        public ApplicationUser applicationUser { get; set; }

        /// <summary>
        /// The unique game the player is playing
        /// </summary>
        public string GameId { get; set; }
    }
}