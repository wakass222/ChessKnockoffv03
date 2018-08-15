<%@ Page Title="" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="Rules.aspx.cs" Inherits="ChessKnockoff.AboutForm" %>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNavTitle" runat="server">
    About
</asp:Content>
<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        //Contains all the code for the board
        function init() {
            //Create the configuration variable for the chessboard
            var cfg = {
                //Disable dragging of pieces
                draggable: false,
                //Set the image files
                pieceTheme: 'Content/Pieces/{piece}.png',
                //Set the pieces up
                position: "rnbqkbnr/pppppppp/8/1PP2PP1/PPPPPPPP/PPPPPPPP/PPPPPPPP/PPPPPPPP"
            };

            //Create the actualy HTML board in the div with the id, board
            board = ChessBoard('board', cfg);
        };

        //Call init once the DOM fully loads
        $(document).ready(init);
    </script>
    <div class="container">
        <div class="row justify-content-center">
            <div id="board" class="col-md-6" style="max-width: 400px">
            </div>
            <div class="col-md-6 ml-1">
                <p>All pieces moves similar to standard chess. In other words, a move is legal if and only if it is legal in standard chess for a similar position. There is an exception for the Pawns.</p>
                <p>The Pieces wins by capturing all the Pawns. This includes pieces promoted from the Pawns.</p>
                <p>The Pawns wins by checkmating the King of the Pieces. Pawns on the first rank may move two squares, similar to Pawns on the second rank. However, Pawns of the Pieces may not capture Pawns on the first rank that has moved two squares, as it is not a valid en passant capture.</p>
                <p>But now with a logistic ELO ranking system so you can realise how bad you are!</p>
            </div>
        </div>
    </div>
</asp:Content>
