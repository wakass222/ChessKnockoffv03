using ChessKnockoff.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ChessKnockoff
{
    public partial class WebForm8 : ExtendedPage
    {
        private void addCellsForRow(int rank, string name, int elo, string css)
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
            tableCellRank.Text = rank.ToString();
            tableRow.Cells.Add(tableCellRank);

            TableCell tableCellUsername = new TableCell();
            //Make the second column the username and escape any tags
            tableCellUsername.Text = HttpUtility.HtmlEncode(name);
            tableRow.Cells.Add(tableCellUsername);

            TableCell tableCellELO = new TableCell();
            //Make the third column the ELO
            tableCellELO.Text = elo.ToString();
            tableRow.Cells.Add(tableCellELO);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Activate the current link
            activateNav("likLeaderboard");

            //Get all the users
            var UsersContext = new ApplicationDbContext();

            //Order them by their ELO in descending order and take the top 10
            ApplicationUser[] orderedUsers = UsersContext.Users.OrderByDescending(appUser => appUser.ELO).ToArray();

            //Check if the user is authenticated, if so search for them
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                //Get the username
                string currentUsername = HttpContext.Current.User.Identity.Name;

                //Store the rank counter
                int rankCounter = 1;

                bool playerInTopTen = false;
                //Loop through each of the top 10 users
                //Will also not add empty cells if there is not enough users
                foreach (ApplicationUser user in orderedUsers.Take(10))
                {
                    //If it is the same person
                    if (user.UserName == currentUsername)
                    {
                        //Show speacial css
                        playerInTopTen = true;
                        addCellsForRow(rankCounter, user.UserName, user.ELO, "table-primary");
                    }
                    else
                    {
                        //Show no extra styling
                        addCellsForRow(rankCounter, user.UserName, user.ELO, "");
                    }

                    //Increment the rank counter
                    rankCounter++;
                }

                //Check if the player is into the top ten
                if (!playerInTopTen)
                {
                    //Find the position of the current player in the ELO rankings
                    int positionOfPlayer = Array.FindIndex(orderedUsers, appUser => appUser.UserName == currentUsername);
                    //Add the user to the end of the table
                    addCellsForRow(positionOfPlayer + 1, currentUsername, orderedUsers[positionOfPlayer].ELO, "table-primary");
                }
            }
            else
            {   
                //Store the rank counter
                int rankCounter = 1;

                //Loop through each of the top 10 users
                //Will also not add empty cells if there is not enough users
                foreach (ApplicationUser user in orderedUsers.Take(10))
                {
                    //Show no extra styling
                    addCellsForRow(rankCounter, user.UserName, user.ELO, "");

                    //Increment the rank counter
                    rankCounter++;
                }

            }
        }
    }
}