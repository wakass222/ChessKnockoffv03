using ChessKnockoff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm8 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Get all the users
            var UsersContext = new ApplicationDbContext();
            //Order them by their ELO in descending order and take the top 10
            var orderedUsers = UsersContext.Users.ToList().OrderByDescending(i => i.ELO).Take(10);

            //Store the rank counter
            int rankCounter = 1;

            //Loop through each of the top 10 users
            //Will also not add empty cells if there is not enough users
            foreach (ApplicationUser user in orderedUsers)
            {
                //Create a new row
                TableRow tableRow = new TableRow();
                //Add the row to the table
                tblLeaderboard.Rows.Add(tableRow);

                //Create a new cell
                TableCell tableCellRank = new TableCell();
                //Make the first column the rank
                tableCellRank.Text = rankCounter.ToString();
                tableRow.Cells.Add(tableCellRank);

                TableCell tableCellUsername = new TableCell();
                //Make the first column the rank
                tableCellUsername.Text = user.UserName;
                tableRow.Cells.Add(tableCellUsername);

                TableCell tableCellELO = new TableCell();
                //Make the first column the rank
                tableCellELO.Text = user.ELO.ToString();
                tableRow.Cells.Add(tableCellELO);

                //Increment the rank counter
                rankCounter++;
            }
        }
    }
}