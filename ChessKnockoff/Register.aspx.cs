using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for the registration page
    /// </summary>
    public partial class RegisterForm : ExtendedPage
    {
        /// <summary>
        /// Custom validation call this method, should not call directly.
        /// </summary>
        /// <param name="source">Object that raised event</param>
        /// <param name="args">Contains event data</param>
        public void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Pass on validation to the password validation function
            checkPasswordMarch(source, args, inpPassword.Value, inpRePassword.Value);
        }

        /// <summary>
        /// Checks whether the email is taken
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>Returns true if the email is taken else false</returns>
        public bool isEmailTaken(string email)
        {
            //Finds a player by that email
            string queryString = "SELECT * FROM Player WHERE Email=@Email";

            //Create the database connection and command then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            using (SqlCommand command = new SqlCommand(queryString, connection))
            {
                //Open the database connection
                connection.Open();

                //Add the username value the query
                command.Parameters.AddWithValue("@Email", email);

                //Execute the command and read the results
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //If there is email already registered
                    if (reader.HasRows)
                    {
                        //Return true
                        return true;
                    }
                    else
                    {
                        //Return false since the email is not taken
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Checks whether the username is taken
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username is taken else false</returns>
        public bool isUsernameTaken(string username)
        {
            //Finds a player by their username
            string queryString = "SELECT * FROM Player WHERE Username=@Username";

            //Create the database connection and command then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            using(SqlCommand command = new SqlCommand(queryString, connection))
            {
                //Open the database connection
                connection.Open();

                //Add the username value the query
                command.Parameters.AddWithValue("@Username", username);

                //Execute the command and store the result
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //If there is user by that name
                    if (reader.HasRows)
                    {
                        //Return true
                        return true;
                    }
                    else
                    {
                        //Return false since the username is not taken
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Create the account
        /// </summary>
        /// <param name="Username">The desired username</param>
        /// <param name="Email">The email</param>
        /// <param name="Password">The plaintext password</param>
        public void createAccount(string username, string email, string passwordPlaintext)
        {
            //Create a new row with player information
            string queryString = "INSERT INTO Player (Username, Email, Password, Salt) VALUES (@Username, @Email, @Password, @Salt)";

            //Create the database connection and command then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            using (SqlCommand command = new SqlCommand(queryString, connection))
            {
                //Open the database connection
                connection.Open();

                //Declare the array to store the salt
                byte[] saltByte = new byte[20];
                //Fill the array with the salt
                fillByteRandom(saltByte);

                //Calculate the salted hash
                byte[] saltedPasswordHash = generateSaltedHash(passwordPlaintext, saltByte);

                //Pass on values to the command
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", saltedPasswordHash);
                command.Parameters.AddWithValue("@Salt", saltByte);

                //Execute the actual command and create the account
                command.ExecuteNonQuery();
            }

            //Overwrite with a new querystring
            queryString = "INSERT INTO Confirmation (Username, ConfirmationToken) VALUES (@Username, @Token)";

            //Create the database connection and command then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            using (SqlCommand command = new SqlCommand(queryString, connection))
            {
                //Open the connection
                connection.Open();

                //Create an array to store 32 bytes
                byte[] randomToken = new byte[32];

                //Fill it with random bytes
                fillByteRandom(randomToken);

                //Add the values to the sql query
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Token", randomToken);

                //Execute the command
                command.ExecuteNonQuery();

                //Create a URI builder to create the path
                UriBuilder builder = new UriBuilder();
                //Set the host
                builder.Host = Request.Url.Host;
                //Set the port
                builder.Port = Request.Url.Port;
                //Set the path
                builder.Path = "/Login";
                //Add the query with the token
                builder.Query += "ConfirmationToken=" + HttpServerUtility.UrlTokenEncode(randomToken);
                //Send the email to the user with the correct link
                sendEmail(email, "Confirm email", "Please confirm your email by clicking <a href=\"" + builder.ToString() + "\">here</a>.");
            }
        }

        /// <summary>
        /// Occurs when the page load. Should not be called directly
        /// </summary>
        /// <param name="sender">Information about the sender</param>
        /// <param name="e">Event arguments</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (isAuthenticated)
            {
                //Redirect them to the play page
                Response.Redirect("~/Play");
            }

            //Make the current link in the navbar active
            activateNav("likRegister");

            //Hide errors as the viewstate is not saved
            altPassword.Visible = false;
            altEmailTaken.Visible = false;
            altUsernameTaken.Visible = false;
            altError.Visible = false;
        }

        /// <summary>
        /// Called by a buttion click, should not be called directly
        /// </summary>
        /// <param name="sender">Information about sender</param>
        /// <param name="e">Event arguments</param>
        protected void RegisterClick(object sender, EventArgs e)
        {
            //Check if controls in the group are all valid
            if (IsValid)
            {
                //If the error list is empty then password is valid else false
                string passwordResult = validatePassword(inpPassword.Value);

                //If the password is not valid
                if (passwordResult != "")
                {
                    //Make sure the alert gets rendered
                    altPassword.Visible = true;
                    //Set the text
                    altPassword.InnerText = passwordResult;
                }

                //Check if the email is taken
                bool emailTaken = isEmailTaken(inpEmail.Value);

                //If it is then show a message
                if (emailTaken)
                {
                    //Show the email is taken message
                    altEmailTaken.Visible = true;
                }

                //Check if the username is taken
                bool usernameTaken = isUsernameTaken(inpUsername.Value);

                //If it is then show a message
                if (usernameTaken)
                {
                    //Show that the email is taken
                    altUsernameTaken.Visible = true;
                }

                //Make sure everything is correct
                if (!emailTaken && !usernameTaken && passwordResult == "")
                {
                    //Create the account
                    createAccount(inpUsername.Value, inpEmail.Value, inpPassword.Value);

                    //Redirect to login screen with the registered flag
                    Response.Redirect("~/Login?Registered=1");
                }
            }
        }
    }
}