using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static System.Web.UI.HtmlControls.HtmlControl;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ChessKnockoff
{
    /// <summary>
    /// Adds validation functions to the Page class and additional functions
    /// </summary>
    public class ExtendedPage: System.Web.UI.Page
    {
        /// <summary>
        /// Exposes master page's control 
        /// </summary>
        /// <param name="controlName">The name of the control to be found</param>
        /// <returns>The nested control</returns>
        public virtual HtmlGenericControl findNestedMasterControl(string controlName)
        {
            ContentPlaceHolder cp = (ContentPlaceHolder)this.Master.Master.FindControl("BaseContentWithHeader");
            return (HtmlGenericControl)cp.FindControl(controlName);
        }

        /// <summary>
        /// Adds the active class to selected control in a master page
        /// </summary>
        /// <param name="ID">The ID of the nav link</param>
        /// <param name="attribute">The class to add</param>
        public void activateNav(string ID)
        {
            //To access nested master page controls, it must be done from the top level
            HtmlGenericControl tempControl = (HtmlGenericControl)findNestedMasterControl(ID);

            //Makes the link active
            tempControl.Attributes["class"] = "nav-item active";
        }

        /// <summary>
        /// Validates passwords
        /// </summary>
        /// <param name="source">The source object from the validation call</param>
        /// <param name="args">Arguments from the validation call</param>
        protected void validatePassword(object source, ServerValidateEventArgs args, string passwordValue, string passwordValueConfirm)
        {
            //Check if passwords match
            bool matchResult = passwordValue == passwordValueConfirm;

            //Only returns true if the password both match
            //Additonal password check is implemented with the identity framework
            if (matchResult)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        /// <summary>
        /// Validates emails
        /// </summary>
        /// <param name="source">The source object from the validation call</param>
        /// <param name="args">Arguments from the validation call</param>
        protected void validateEmail(object source, ServerValidateEventArgs args)
        {
            //Will only return true if the email has not been taken and is valid, also checks its length
            Regex regexEmail = new Regex(@"^(([^<>()\[\]\\.,;:\s@]+(\.[^<>()\[\]\\.,;:\s@]+)*)|(.+))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$", RegexOptions.IgnoreCase);
            bool regexEmailResult = regexEmail.IsMatch(args.Value);

            if (regexEmailResult)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        /// <summary>
        /// Validates username
        /// </summary>
        /// <param name="source">The source object from the validation call</param>
        /// <param name="args">Arguments from the validation call</param>
        protected void validateUsername(object source, ServerValidateEventArgs args)
        {
            //Create username regex
            Regex regexUsername = new Regex(@"^[a-zA-Z0-9_]*$");
            bool regexUsernameResult = regexUsername.IsMatch(args.Value);

            //Check if the username only contains alphanumeric values and is 25 characters or less
            if (regexUsernameResult && args.Value.Length <= 25)
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }
    }

    /// <summary>
    /// Holds the functions for if the page is a child of the required login master page
    /// </summary>
    public class LoginRequiredPage : ExtendedPage
    {
        /// <summary>
        /// Exposes master page's control and overrides the base class
        /// </summary>
        /// <param name="controlName">The name of the control to be found</param>
        /// <returns>The nested control</returns>
        public override HtmlGenericControl findNestedMasterControl(string controlName)
        {
            ContentPlaceHolder cp = (ContentPlaceHolder)this.Master.Master.Master.FindControl("BaseContentWithHeader");
            return (HtmlGenericControl)cp.FindControl(controlName);
        }
    }
}