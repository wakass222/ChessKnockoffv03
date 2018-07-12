using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static ChessKnockoff.Utilities;
using static System.Web.UI.HtmlControls.HtmlControl;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace ChessKnockoff
{
    public static class Validation
    {
        /// <summary>
        /// Validates passwords
        /// </summary>
        /// <param name="source">The source object from the validation call</param>
        /// <param name="args">Arguments from the validation call</param>
        public static void validatePassword(object source, ServerValidateEventArgs args, string passwordValue, string passwordValueConfirm)
        {
            //Check if passwords match
            bool matchResult = passwordValue == passwordValueConfirm;

            //Only returns true if the password both match and is of the correct length
            if (matchResult && passwordValue.Length)
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
        public static void validateEmail(object source, ServerValidateEventArgs args)
        {
            //Will only return true if the email has not been taken and is valid
            if (IsValidEmail(args.Value))
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
        public static void validateUsername(object source, ServerValidateEventArgs args)
        {
            //Create username regex
            Regex regexUsername = new Regex(@"^[a-zA-Z0-9_]*$");
            bool regexUsernameResult = regexUsername.IsMatch(args.Value);

            //Will only return true if the username is valid and has not beent taken
            if (regexUsernameResult)
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