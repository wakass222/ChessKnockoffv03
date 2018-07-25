using ChessKnockoff.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm8 : ExtendedPage
    {
        private void addCellsForRow(string rank, string name, string elo, string css)
        {
            //Create a new row
            TableRow tableRow = new TableRow();

            //Set the styling on the row
            tableRow.CssClass = css;

            //Add the row to the table
            tblLeaderboard.Rows.Add(tableRow);

            //Create a new cell
            TableCell tableCellRank = new TableCell();
            //Make the first column the rank
            tableCellRank.Text = rank;
            tableRow.Cells.Add(tableCellRank);

            TableCell tableCellUsername = new TableCell();
            //Make the second column the username and escape any tags
            tableCellUsername.Text = HttpUtility.HtmlEncode(name);
            tableRow.Cells.Add(tableCellUsername);

            TableCell tableCellELO = new TableCell();
            //Make the third column the ELO
            tableCellELO.Text = elo;
            tableRow.Cells.Add(tableCellELO);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Activate the current link
            activateNav("likLeaderboard");

            //Check if the user is authenticated, if so search for them
            if (isAuthenticated())
            {
                //Get all the users and order them by ELO is decending order
                string queryString = "SELECT TOP 10 ROW_NUMBER() OVER (ORDER BY ELO DESC) AS Rank, Username, ELO FROM Player";

                //Stores whether the play is in the top 10
                bool playerInTopTen = false;

                //Create the database connection and command then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    //Open the database connection
                    connection.Open();

                    //Execute the command
                    SqlDataReader reader = command.ExecuteReader();

                    //Loop through each row
                    foreach (DbDataRecord row in reader)
                    {
                        //If it is the same person
                        if (row["Username"].ToString() == Session["Username"].ToString())
                        {
                            //Show speacial css
                            playerInTopTen = true;
                            addCellsForRow(row["Rank"].ToString(), row["Username"].ToString(), row["ELO"].ToString(), "table-primary");
                        }
                        else
                        {
                            //Show no extra styling
                            addCellsForRow(row["Rank"].ToString(), row["Username"].ToString(), row["ELO"].ToString(), "");
                        }
                    }
                }

                //Check if the player is into the top ten
                if (!playerInTopTen)
                {
                    //Finds the rank of the player
                    queryString = "WITH OrderedPlayer AS (SELECT Username, ELO, ROW_NUMBER() OVER (ORDER BY ELO DESC) AS Rank FROM Player) SELECT * FROM OrderedPlayer WHERE Username=@Username";

                    //Create the database connection and command then dispose when done
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        //Open the database connection
                        connection.Open();

                        //Add the parameter
                        command.Parameters.AddWithValue("@Username", Session["Username"]);

                        //Execute the command
                        SqlDataReader reader = command.ExecuteReader();

                        //Read the first row
                        reader.Read();

                        //Add the user to the end of the table
                        addCellsForRow(reader["Rank"].ToString(), reader["Username"].ToString(), reader["ELO"].ToString(), "table-primary");
                    }
                }
            }
            else
            {
                //Select the top 10 players with their rank
                string queryString = "SELECT TOP 10 ROW_NUMBER() OVER (ORDER BY ELO DESC) AS Rank, Username, ELO FROM Player";

                //Create the database connection and command then dispose when done
                using (SqlConnection connection = new SqlConnection(dbConnectionString))
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    //Open the database connection
                    connection.Open();

                    //Execute the command
                    SqlDataReader reader = command.ExecuteReader();

                    //Loop through each row
                    foreach (DbDataRecord row in reader)
                    {
                        //Show no extra styling
                        addCellsForRow(row["Rank"].ToString(), row["Username"].ToString(), row["ELO"].ToString(), "");
                    }
                }
            }
        }
    }
}