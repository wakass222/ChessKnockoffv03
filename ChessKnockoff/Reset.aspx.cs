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
            validatePassword(source, args, inpPassword.Value, inpRePassword.Value);
        }

        /// <summary>
        /// Checks whether the token is valid or not
        /// </summary>
        /// <param name="tokenID">The token from the url</param>
        /// <returns>Returns true if the token is correct else returns false</returns>
        public bool isResetTokenCorrect(string token)
        {
            //Stores the query string
            string queryString = "SELECT * FROM Reset INNER JOIN Player ON Reset.Username = Player.Username AND ResetToken=@ResetToken";

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand sqlCommand = new SqlCommand(queryString, connection);

                try
                {
                    //Add the sql parameter
                    sqlCommand.Parameters.AddWithValue("@ResetToken", decodeToBytes(token));

                    //Execute the sql command
                    SqlDataReader reader = sqlCommand.ExecuteReader();

                    //If the reader has rows then the token is correct
                    if (reader.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        //Return false since the reset token is incorrect
                        return false;
                    }
                }
                catch (Exception)
                {
                    //Return false since the token was not in the correct format
                    return false;
                    throw;
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
            if (IsValid)
            {
                //Get the reset code
                string code = Request.QueryString["ResetToken"];

                //Re-check the reset token in case it was altered
                if (isResetTokenCorrect(code))
                {
                    //Store the list of errors
                    string errorList = validatePassword(inpPassword.Value);

                    //Check error list is empty
                    if (errorList != "")
                    {
                        //Show the error message
                        altError.Visible = true;
                        //Show the errors
                        altError.InnerText = errorList;
                    }
                    else
                    {
                        //There was no error so change the password

                        //Create a byte array to store the salt
                        byte[] newSalt = new byte[20];
                        //Fill the array with the salt
                        fillByteRandom(newSalt);

                        //Hash the new password with the salt
                        byte[] saltedHash = generateSaltedHash(inpPassword.Value, newSalt);

                        //Create the database connection then dispose when done
                        using (SqlConnection connection = new SqlConnection(dbConnectionString))
                        {
                            //Open the database connection
                            connection.Open();

                            //Stores the query string
                            string queryString = "UPDATE Player SET Password=@Password, Salt=@Salt INNER JOIN Reset ON Player.Username = Reset.Username AND ResetToken=@ResetToken";

                            //Create the query string in the sqlCommand format
                            SqlCommand command = new SqlCommand(queryString, connection);

                            //Add the parameters to the command
                            command.Parameters.AddWithValue("@Salt", saltedHash);
                            command.Parameters.AddWithValue("@Password", saltedHash);
                            command.Parameters.AddWithValue("@ResetToken", decodeToBytes(code));

                            //Execute the statement
                            command.ExecuteNonQuery();

                            //Redirect to the login page and show the success message
                            Response.Redirect("~/Login?ResetPassword=1");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called on page load. Should not be called directly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the error message
            altError.Visible = false;

            //Get the reset code
            string code = Request.QueryString["ResetToken"];

            //Check if the reset code is valid
            bool isValid = isResetTokenCorrect(code);

            //Check if the code is invalid
            if (!isValid)
            {
                //Redirect them to the play game but if they arent logged in, they are redirected to the login page
                Response.Redirect("~/Play");
            }
        }
    }
}