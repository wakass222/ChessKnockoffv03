using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ChessKnockoff
{
    public class GameHub : Hub
    {
        public void movePiece()
        {
            Clients.All.logMessage("Hello");
        }
    }
}