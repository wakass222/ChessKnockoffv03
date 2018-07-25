using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChessDotNet;
using System.Security.Principal;

namespace ChessKnockoff.Models
{
    public class playerConnection
    {
        /// <summary>
        /// Creates a new player connection object
        /// </summary>
        /// <param name="applicationUser">The Identity of the user</param>
        /// <param name="connectionString"></param>
        public playerConnection(string Username, string connectionString)
        {
            //Setup the new object
            this.Username = Username;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Which side the player is: black or white
        /// </summary>
        public Player side { get; set; }

        /// <summary>
        /// The Username of the player
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Stores the connection ID of the client
        /// </summary>
        public string connectionString { get; set; }

        /// <summary>
        /// The unique game the player is playing
        /// </summary>
        public string GameId { get; set; }
    }
}