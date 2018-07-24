using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace ChessKnockoff
{
    public partial class WebForm4 : ExtendedPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //If user is already logged in
            if (isAuthenticated())
            {
                //Check if they had the return query
                if (this.Request.QueryString["ReturnUrl"] != null)
                {
                    //IF they did redirect them to that
                    this.Response.Redirect(Request.QueryString["ReturnUrl"].ToString());
                }
                else
                {
                    //Redirect them to the play page
                    this.Response.Redirect("/Play");
                }
            }

            //Make the current link in the navbar active
            activateNav( "likLogin");

            //Hide the messages when first entering the page and for other request as the viewstate is not saved
            altAuthentication.Visible = false;
            altRegistered.Visible = false;
            altMustBeLoggedIn.Visible = false;
            altVerify.Visible = false;
            altEmailConfirm.Visible = false;
            altResetPassword.Visible = false;
            altLockout.Visible = false;
            
            //If the user was redirected from another page that required to be logged in
            if (Request.QueryString["ReturnUrl"] != null)
            {
                //Display the message
                altMustBeLoggedIn.Visible = true;
            }

            //If the user has a confirmation code
            if (Request.QueryString["Token"] != null)
            {
                //Check the password confirmation token

                //Stores the query string
                string queryString = "UPDATE Player SET Player.EmailIsConfirmed = 1 FROM Player INNER JOIN Reset ON Player.Username = Reset.Username WHERE Reset.Token = @Token";

                //Create the database connection then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                {
                    //Open the database connection
                    connection.Open();

                    //Create the query string in the sqlCommand format
                    SqlCommand command = new SqlCommand(queryString, connection);

                    //Add the parameters
                    command.Parameters.AddWithValue("@Token", Request.QueryString["Token"]);

                    //Store the result in a reader
                    int rowsAffected = command.ExecuteNonQuery();

                    //Rows were affected therefore show the success message
                    if (rowsAffected > 0)
                    {
                        //Show that the email has been confirmed
                        altEmailConfirm.Visible = true;
                    }
                }
            }

            //If the user just registered show them the success message and prompt them to login
            if (Request.QueryString["Registered"] == "1")
            {
                //Displat the message
                altRegistered.Visible = true;
            }

            //If the password was reset
            if (Request.QueryString["ResetPassword"] == "1")
            {
                //Displat the message
                altResetPassword.Visible = true;
            }
        }

        /// <summary>
        /// Validates the user credentials
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="passwordPlaintext">The password is plaintext</param>
        private bool validateUser(string username, string passwordPlaintext)
        {
            //Stores the query string
            string queryString = "SELECT * FROM Player WHERE Username=@Username";

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

                //Check if a user was found
                if (reader.HasRows)
                {
                    //Read the first row
                    reader.Read();

                    //Retrieve the salt password from the database and cast it to a byte array
                    byte[] salt = (byte[])reader["Salt"];

                    //Retrieve the salted hash from the database
                    byte[] saltedPasswordHash = (byte[])reader["Password"];

                    //Calculate user entered hash with the salt
                    byte[] saltedUserenteredHash = generateSaltedHash(passwordPlaintext, salt);

                    //Check if they match
                    if (saltedUserenteredHash.SequenceEqual(saltedPasswordHash))
                    {
                        //Return true that the crendentials are valid
                        return true;
                    }
                    else
                    {
                        //Show the fail message
                        altAuthentication.Visible = true;

                        //The crendentials are not valid so return false
                        return false;
                    }
                } else
                {
                    //Show an error
                    altAuthentication.Visible = true;

                    //The crendentials are not valid so return false
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks whether the user has their email confirmed or not
        /// </summary>
        /// <param name="username">The name of the player</param>
        /// <returns>Returns true if their email is confirmed else false</returns>
        private bool isEmailConfirmed(string username)
        {
            //Stores the query string
            string queryString = "SELECT * FROM Player WHERE Username=@Username AND EmailIsConfirmed=1";

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand command = new SqlCommand(queryString, connection);

                //Add the parameters to the query
                command.Parameters.AddWithValue("@Username", username);

                //Execute the command and store how many rows it found
                int rowsAffected = command.ExecuteNonQuery();

                //If there were no rows matching it
                if (rowsAffected == 0)
                {
                    //Show that the email was not confirmed
                    altEmailConfirm.Visible = true;

                    //Return false since their email was not confirmed
                    return false;
                } else
                {
                    //Return true since their email is confirmed
                    return true;
                }
            }
        }

        protected void LoginClick(object sender, EventArgs e)
        {
            if (IsValid)
            {
                //Validate their credentials
                if (validateUser(inpUsername.Value, inpPassword.Value))
                {
                    //Store their information in a session variable
                    Session["Username"] = inpUsername.Value;

                    //Redirect them to the play page
                    Response.Redirect("/Play");
                }
            }
        }
    }
}