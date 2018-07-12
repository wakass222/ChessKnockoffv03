using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class BaseWithScripts_MustBeLoggedIn : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
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
}