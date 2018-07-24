using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm7 : ExtendedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the success message as the viewstate is not saved
            altEmailSent.Visible = false;
            altEmailFail.Visible = false;

            //If user is already logged in
            if (isAuthenticated())
            {
                //Redirect them to the play page
                Response.Redirect("~/Play");
            }
        }

        protected void EmailClick(object sender, EventArgs e)
        {
            //Check if the inputs are valid
            if (IsValid)
            {
                //If the email is confirmed
                if (isEmailConfirmed(inpEmail.Value) == true)
                {
                    //Holds the random bytes
                    byte[] randomToken = new byte[32];
                    //FIll it with random bytes
                    fillByteRandom(randomToken);

                    //Create a URI builder
                    UriBuilder builder = new UriBuilder();
                    //Set the host
                    builder.Host = Request.Url.Host;
                    //Set the port
                    builder.Port = Request.Url.Port;
                    //Set the path
                    builder.Path = "/Reset/";
                    //Set the query parameter
                    builder.Query += "ResetToken=" + encodeToString(randomToken);

                    //Send the email
                    sendEmail(inpEmail.Value, "Reset password", "Please reset your password by clicking <a href=\"" + builder.ToString() + "\">here</a>.");

                    //Show that it was successful
                    altEmailSent.Visible = true;
                }
                else
                {
                    //Show that it was unsuccessful
                    altEmailFail.Visible = true;
                }
            }
        }
    }
}