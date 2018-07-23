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
    /// 
    /// </summary>
    public partial class ResetForm : ExtendedPage
    {
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
            string queryString = "SELECT * FROM ResetToken INNER JOIN Player ON ResetToken.Id = Player.Id AND ResetToken=@ResetToken";

            //Create the reader to store results
            SqlDataReader reader;

            //Create the database connection then dispose when done
            using (SqlConnection connection = new SqlConnection(dbConnectionString))
            {
                //Open the database connection
                connection.Open();

                //Create the query string in the sqlCommand format
                SqlCommand sqlCommand = new SqlCommand(queryString, connection);

                //Add the sql parameter
                sqlCommand.Parameters.AddWithValue("@ResetToken", token);

                //Execute the sql command
                reader = sqlCommand.ExecuteReader();

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
        }

        protected void ResetPassword(object sender, EventArgs e)
        {
            //Check if the inputs are valid in this case if both passwords match
            if (IsValid)
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

                    //Redirect to the login page and show the success message
                    Response.Redirect("~/Login?ResetPassword=1");
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Hide the error message
            altError.Visible = false;

            //Get the reset code
            string code = Request.QueryString["ResetToken"];

            //Check if the code and userID is sent
            if (code == null && userID != "")
            {
                //Redirect them to the play game but if they arent logged in, they are redirected to the login page
                Response.Redirect("~/Play");
            }
        }
    }
}