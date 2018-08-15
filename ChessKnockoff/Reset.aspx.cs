using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ChessKnockoff
{
    /// <summary>
    /// Class for reset page
    /// </summary>
    public partial class ResetForm : ExtendedPage
    {
        /// <summary>
        /// Custom validation calls this function. Should not be called directly
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void checkPassword(object source, ServerValidateEventArgs args)
        {
            //Pass on password validation to another function
            checkPasswordMarch(source, args, inpPassword.Value, inpRePassword.Value);
        }

        /// <summary>
        /// Checks whether the token is valid or not
        /// </summary>
        /// <param name="tokenID">The token from the url</param>
        /// <returns>Returns true if the token is correct else returns false</returns>
        public bool isResetTokenCorrect(byte[] token)
        {
            //Finds the reset token and the associated user
            string queryString = "SELECT * FROM Reset INNER JOIN Player ON Reset.Username = Player.Username AND ResetToken=@ResetToken";

            //Create the database connection and command then dispose when done
            using (SqlConnection connectionSelect = new SqlConnection(dbConnectionString))
            using (SqlCommand commandSelect = new SqlCommand(queryString, connectionSelect))
            {
                //Open the database connection
                connectionSelect.Open();

                //Add the sql parameter
                commandSelect.Parameters.AddWithValue("@ResetToken", token);

                //Execute the sql command
                using (SqlDataReader reader = commandSelect.ExecuteReader())
                {
                    //If the reader has rows then the token is correct
                    if (reader.HasRows)
                    {
                        //Get the values of the first row
                        reader.Read();

                        //Check if it has not expired
                        if ((DateTime)reader["ExpirationDateUTC"] > DateTime.UtcNow)
                        {
                            return true;
                        }
                        else
                        {
                            //Delete it since it is not required

                            //Store a new query string
                            queryString = "DELETE FROM Reset WHERE ResetToken=@ResetToken";

                            //Create the database connection and command then dispose when done
                            using (SqlConnection connectionDelete = new SqlConnection(dbConnectionString))
                            using (SqlCommand commandDelete = new SqlCommand(queryString, connectionDelete))
                            {
                                //Open the database connection
                                connectionDelete.Open();

                                //Add the parameters to the command
                                commandDelete.Parameters.AddWithValue("@ResetToken", token);

                                //Execute the statement
                                commandDelete.ExecuteNonQuery();
                            }

                            //Redirect the forgot page and show that it has expired
                            Response.Redirect("~/Forgot?TokenExpired=1");

                            return false;
                        }
                    }
                    else
                    {
                        //Return false since the reset token is incorrect
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// Changes the password, called by button and should not be called directly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ResetPassword(object sender, EventArgs e)
        {
            //Check if the inputs are valid in this case if both passwords match
            //Using the URL decode on an empty argument will throw and error
            if (IsValid && Request.QueryString["ResetToken"] != null)
            {
                //Get the reset code in a string
                byte[] code = HttpServerUtility.UrlTokenDecode(Request.QueryString["ResetToken"]);

                //Create a byte array to store the salt
                byte[] newSalt = new byte[20];
                //Fill the array with the salt
                fillByteRandom(newSalt);

                //Hash the new password with the salt
                byte[] newSaltedHash = generateSaltedHash(inpPassword.Value, newSalt);

                //Updates the user's password
                string queryString = "UPDATE Player SET Password=@Password, Salt=@Salt FROM Player INNER JOIN Reset ON Player.Username = Reset.Username WHERE Reset.ResetToken = @ResetToken";

                int rowsAffected;

                //Create the database connection and command then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    //Open the database connection
                    connection.Open();

                    //Add the parameters to the command
                    command.Parameters.AddWithValue("@Salt", newSalt);
                    command.Parameters.AddWithValue("@Password", newSaltedHash);
                    command.Parameters.AddWithValue("@ResetToken", code);

                    //Execute the statement
                    rowsAffected = command.ExecuteNonQuery();
                }

                //Delete it since it has been used
                queryString = "DELETE FROM Reset WHERE ResetToken=@ResetToken";

                //Create the database connection and command then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    //Open the database connection
                    connection.Open();

                    //Add the parameters to the command
                    command.Parameters.AddWithValue("@ResetToken", code);

                    //Execute the statement
                    command.ExecuteNonQuery();
                }

                //Redirect to the login page and show the success message
                Response.Redirect("~/Login?ResetPassword=1");
            }
        }

        /// <summary>
        /// Called on page load. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Check if the reset code is valid, must check before hand if it is null to not throw an error
            bool isValid = Request.QueryString["ResetToken"] != null && isResetTokenCorrect(HttpServerUtility.UrlTokenDecode(Request.QueryString["ResetToken"]));

            //Check if the code is invalid
            if (!isValid)
            {
                //Redirect them to the login
                Response.Redirect("~/Login");
            }
        }
    }
}