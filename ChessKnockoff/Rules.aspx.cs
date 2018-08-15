using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for about page
    /// </summary>
    public partial class AboutForm : ExtendedPage
    {
        /// <summary>
        /// Called when the page loads. Should not be called directly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Activate the current page
            activateNav("likAbout");
        }
    }
}