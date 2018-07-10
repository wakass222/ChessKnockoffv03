using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    /// <summary>
    /// Contains any utilities
    /// </summary>
    static public class Utilities
    {
        /// <summary>
        /// Checks if email is valid
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns></returns>
        static public bool IsValidEmail(string email)
        {
            try
            {
                //See if a MailAddress object can be created
                var addr = new System.Net.Mail.MailAddress(email);

                //Will return true
                return addr.Address == email;
            }
            catch //An error was thrown
            {
                //Return false
                return false;
            }
        }

        /// <summary>
        /// Exposes master page's control 
        /// </summary>
        /// <param name="self">The current page</param>
        /// <param name="controlName">The name of the control to be found</param>
        /// <returns>The nested control</returns>
        static public HtmlGenericControl findNestedMasterControl(Page self, string controlName)
        {
            ContentPlaceHolder cp = (ContentPlaceHolder)self.Master.Master.FindControl("BaseContentWithHeader");
            return (HtmlGenericControl)cp.FindControl(controlName);
        }

        /// <summary>
        /// Adds the attribute to selected control's class in a master page
        /// </summary>
        /// <param name="self">The current page</param>
        /// <param name="ID">The ID of the nav link</param>
        /// <param name="attribute">The class to add</param>
        static public void activateNav (Page self, string ID)
        {
            //To access nested master page controls, it must be done from the top level
            HtmlGenericControl tempControl = (HtmlGenericControl)findNestedMasterControl(self, ID);

            //Makes the link active
            tempControl.Attributes["class"] = "nav-item active";
        }

        /// <summary>
        /// Retrieves the control that caused the postback.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        static public Control GetControlThatCausedPostBack(Page page)
        {
            //initialize a control and set it to null
            Control ctrl = null;

            //get the event target name and find the control
            string ctrlName = page.Request.Params.Get("__EVENTTARGET");
            if (!String.IsNullOrEmpty(ctrlName))
                ctrl = page.FindControl(ctrlName);

            //return the control to the calling method
            return ctrl;
        }
    }
}