using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChessDotNet;
using System.Security.Principal;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ChessKnockoff.Models
{
    public class playerConnection
    {
        /// <summary>
        /// Creates a new player connection object
        /// </summary>
        /// <param name="applicationUser">The Identity of the user</param>
        /// <param name="connectionString"></param>
        public playerConnection(IIdentity applicationIdentity, string connectionString)
        {
            //The current user information can not be accessed by using the OWIN context
            //Therefore a connection has to be made manually
            ApplicationDbContext mycontext = new ApplicationDbContext();
            UserStore<ApplicationUser> mystore = new UserStore<ApplicationUser>(mycontext);
            ApplicationUserManager userMgr = new ApplicationUserManager(mystore);

            //Look for the user and their data
            ApplicationUser foundUser = userMgr.FindByName(applicationIdentity.Name);

            //Setup the new object
            this.userInformation = foundUser;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Which side the player is: black or white
        /// </summary>
        public Player side { get; set; }

        /// <summary>
        /// The Identity of the player with all their data
        /// </summary>
        public ApplicationUser userInformation { get; set; }

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