using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for the BaseWithHEaderNavLogin master page
    /// </summary>
    public partial class BaseWithHeaderNavLogin : System.Web.UI.MasterPage
    {
        /// <summary>
        /// Called when the page loads. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //If the user is not authenticated
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Then redirect to the login page with reference to the current page
                string OriginalUrl = HttpContext.Current.Request.RawUrl;
                string LoginPageUrl = "/login";
                HttpContext.Current.Response.Redirect(String.Format("{0}?ReturnUrl={1}", LoginPageUrl, OriginalUrl));
            }
        }
    }
}