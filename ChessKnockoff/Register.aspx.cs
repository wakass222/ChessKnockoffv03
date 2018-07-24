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
            validatePassword(source, args, inpPassword.Value, inpRePassword.Value);
        }

        /// <summary>
        /// Checks whether the email is taken
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>Returns true if the email is taken else false</returns>
        public bool isEmailTaken(string email)
        {
            //Stores the query string
            string queryString = "SELECT * FROM Player WHERE Email=@Email";

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand sqlCommand = new SqlCommand(queryString, connection);

                //Add the username value the query
                sqlCommand.Parameters.AddWithValue("@Email", email);

                //Since a single row/column is returned only necessary to get first one
                //Returns the amount of users by that name
                SqlDataReader reader = sqlCommand.ExecuteReader();

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

        /// <summary>
        /// Checks whether the username is taken
        /// </summary>
        /// <param name="username">The username to check</param>
        /// <returns>True if the username is taken else false</returns>
        public bool isUsernameTaken(string username)
        {
            //Stores the query string
            string queryString = "SELECT * FROM Player WHERE Username=@Username";

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand sqlCommand = new SqlCommand(queryString, connection);

                //Add the username value the query
                sqlCommand.Parameters.AddWithValue("@Username", username);

                //Since a single row/column is returned only necessary to get first one
                //Returns the amount of users by that name
                SqlDataReader reader = sqlCommand.ExecuteReader();

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

        /// <summary>
        /// Create the account
        /// </summary>
        /// <param name="Username">The desired username</param>
        /// <param name="Email">The email</param>
        /// <param name="Password">The plaintext password</param>
        public void createAccount(string username, string email, string passwordPlaintext)
        {
            //Stores the query string
            string queryString = "INSERT INTO Player (Username, Email, Password, Salt) VALUES (@Username, @Email, @Password, @Salt)";

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

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

                //Create a new query string
                queryString = "INSERT INTO Reset (Username, Token) VALUES (@Username, @Token)";

                //Overwrite with a new command
                command = new SqlCommand(queryString, connection);

                //Create an array to store 32 bytes
                byte[] randomToken = new byte[32];

                //Fill it with random bytes
                fillByteRandom(randomToken);

                //Add the values to the sql query
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Token", Convert.ToBase64String(randomToken));

                //Create a URI builder to create the path
                UriBuilder builder = new UriBuilder();
                //Set the host
                builder.Host = Request.Url.Host;
                //Set the port
                builder.Port = Request.Url.Port;
                //Set the path
                builder.Path = "/Login/";
                //Add the query with the token
                builder.Query += "Token=" + Convert.ToBase64String(randomToken);
                //Send the email to the user with the correct link
                sendEmail(email, "Please click here to confirm email: " + "< a href =" + builder.ToString() + "\">here</a>");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (User.Identity.IsAuthenticated)
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

        protected void RegisterClick(object sender, EventArgs e)
        {
            //Check if controls in the group are all valid
            if (IsValid)
            {
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

                //Make sure all vairables have not be taken
                if (!emailTaken && !usernameTaken)
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