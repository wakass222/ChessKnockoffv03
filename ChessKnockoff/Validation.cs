using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static ChessKnockoff.Utilities;
using static System.Web.UI.HtmlControls.HtmlControl;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ChessKnockoff
{
    /// <summary>
    /// Adds validation functions to the Page class
    /// </summary>
    public class ValidationPage: System.Web.UI.Page
    {
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
}