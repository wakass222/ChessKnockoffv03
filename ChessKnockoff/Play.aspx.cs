using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChessDotNet;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for the play page
    /// </summary>
    public partial class PlayForm : LoginRequiredPage
    {
        /// <summary>
        /// Called when the page loads. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
           //ACtivate the link in the navbar
            activateNav("likPlay");
        }
    }
}