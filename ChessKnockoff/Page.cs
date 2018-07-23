using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using static System.Web.UI.HtmlControls.HtmlControl;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Security.Cryptography;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Configuration;
using System.Text;
using System.Net;

namespace ChessKnockoff
{
    /// <summary>
    /// Adds validation functions to the Page class and additional functions
    /// </summary>
    public class ExtendedPage : System.Web.UI.Page
    {
        /// <summary>
        /// Stores the database connection string and can not be altered
        /// </summary>
        public static string dbConnectionString {get; } = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database.mdf;Integrated Security=True";

        /// <summary>
        /// Validates a password
        /// </summary>
        /// <param name="password">The password to validate</param>
        /// <returns>A string of errors or "" if there was no error</returns>
        public string validatePassword(string password)
        {
            //Create a string to store the list of errors
            string error = "";

            //Check if the password is too short
            if (password.Length <= 6)
            {
                error += "Password must be larger than 6 characters. ";
            }

            //Check if the password is not too long
            if (password.Length >= 256)
            {
                error += "Password must be shorter than 256 characters. ";
            }

            //Check if the password contains an upper case letter
            if (password.All(char.IsUpper))
            {
                error += "Password must contain upper case character. ";
            }

            //Check if the password contains a number
            if (!password.All(char.IsNumber))
            {
                error += "Password must contain a number. ";
            }

            //Check if the password has a punctuation
            if (!password.All(Char.IsPunctuation))
            {
                error += "Password must have a punctuation mark. ";
            }

            //Return the error
            return error;
        }

        /// <summary>
        /// Sends an email the address
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        public void sendEmail(string destination, string message)
        {
            //Create mail message object
            using (MailMessage mail = new MailMessage())
            {
                //Set the sending email address
                mail.From = new MailAddress(ConfigurationManager.AppSettings["emailServiceUserName"]);
                //Set the destination address
                mail.To.Add(destination);
                //Set the subject
                mail.Subject = "Chess Knockoff Verification";
                //Set the body
                mail.Body = message;
                //Make sure the body is rendered as HTML
                mail.IsBodyHtml = true;

                //Create a SmtpClient to gmail servers and the dispose when finished
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    //Credentials for verification
                    smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailServiceUserName"], ConfigurationManager.AppSettings["emailServicePassword"]);
                    //SSL must be enabled
                    smtp.EnableSsl = true;
                    //Send the email and wait for it to finish
                    smtp.SendMailAsync(mail).Wait();
                }
            }
        }

        /// <summary>
        /// Checks whether the email is taken
        /// </summary>
        /// <param name="email">The email to check</param>
        /// <returns>Returns true if the email is taken else false</returns>
        public bool isEmailTaken(string email)
        {
            //Stores the query string
            string queryString = "SELECT COUNT(*) FROM Player WHERE Email=@Email";

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
                int emailExists = (int)sqlCommand.ExecuteScalar();

                //If there is email already registered
                if (emailExists > 0)
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
            string queryString = "SELECT COUNT(*) FROM Player WHERE Username=@Username";

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
                int userExists = (int)sqlCommand.ExecuteScalar();

                //If there is user by that name
                if (userExists > 0)
                {
                    //Return true
                    return true;
                } else
                {
                    //Return false since the username is not taken
                    return false;
                }
            }
        }

        /// <summary>
        /// Validates the user credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="passwordPlaintext">The password is plaintext</param>
        public bool validateUser(string username, string passwordPlaintext)
        {
            //Stores the query string
            string queryString = "SELECT Salt FROM Player WHERE Username=@Username";

            //Create the reader to store results
            SqlDataReader reader;

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Add the parameters into the query
                command.Parameters.AddWithValue("@Username", username);

                //Execute the SQL command and get the results
                reader = command.ExecuteReader();

                //Retrieve the salt password from the database and cast it to a byte array
                byte[] salt = (byte[])reader["Salt"];

                //Retrieve the salted hash from the database
                byte[] saltedPasswordHash = (byte[])reader["Password"];

                //Calculate user entered hash with the salt
                byte[] saltedUserenteredHash = generateSaltedHash(passwordPlaintext, salt);

                //Check if they match
                if (saltedUserenteredHash == saltedPasswordHash)
                {
                    //Return true that the crendentials are valid
                    return true;
                } else
                {
                    //The crendentials are not valid so return false
                    return false;
                }
            }
        }

        /// <summary>
        /// Confirms the email
        /// </summary>
        /// <param name="UsernameID">The ID from the querystring to confirm</param>
        /// <returns>True if the email was confirmed else false</returns>
        public bool confirmEmail(string UsernameID)
        {
            //Stores the query string
            string queryString = "UPDATE Player SET EmailIsConfirmed=1 WHERE Id=@Id";

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Execute the command and store how many rows were affected
                int rowsAffected = command.ExecuteNonQuery();

                //If one row was altered then it was succesful
                if (rowsAffected == 1)
                {
                    //Return true since it was successful
                    return true;
                } else
                {
                    //Return false since it did not work
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
            string queryString = "INSERT INTO Player (Username, Email, Password, Salt) VALUES (@Username, @Email, @Password, @Salt) SELECT SCOPE_IDENTITY()";
            //Create the random number generator object
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            //Create the array to store 20 bytes of random salt
            byte[] saltByte = new byte[20];

            //Fill the array with random bytes
            rng.GetBytes(saltByte);

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Calculate the salted hash
                byte[] saltedPasswordHash = generateSaltedHash(passwordPlaintext, saltByte);

                //Pass on values to the command
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", saltedPasswordHash);
                command.Parameters.AddWithValue("@Salt", saltByte);

                //Execute the actual command
                //Store the ID of the newly created user
                int createdUserID = (int)command.ExecuteScalar();

                //Create base uri
                Uri baseUri = new Uri(Request.Url.Host);
                //Add on the confirmation query string
                Uri finalUri = new Uri(baseUri, "/Login/?Email=" + createdUserID);
                //Send the email to the user with the correct link
                sendEmail(email, "Please click here to confirm email: " + "< a href =" + finalUri + "\">here</a>");

            }
        }

        /// <summary>
        /// Calculates the salted hash
        /// </summary>
        /// <param name="plainText">The plaintext of the password</param>
        /// <param name="salt">The salt</param>
        /// <returns>The computed salted hash</returns>
        static byte[] generateSaltedHash(string plainText, byte[] salt)
        {
            //Convert the string password to bytes
            byte[] passwordBytes = Encoding.UTF8.GetBytes(plainText);

            //Create the algorithm object
            HashAlgorithm algorithm = new SHA256Managed();

            //Create an array to store the bytes of the password
            byte[] plainTextSalted = new byte[plainText.Length + salt.Length];

            //Loop through each byte of the plaintext, copying the original bytes
            for (int i = 0; i < plainText.Length; i++)
            {
                //Add the plaintext to the beginning of the array
                plainTextSalted[i] = passwordBytes[i];
            }

            //Loop through each byte of the salt, adding it to the array after the plaintext part
            for (int i = 0; i < salt.Length; i++)
            {
                //Add the salt bytes after the plain text bytes
                plainTextSalted[plainText.Length + i] = salt[i];
            }

            //Return the result
            return algorithm.ComputeHash(plainTextSalted);
        }

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