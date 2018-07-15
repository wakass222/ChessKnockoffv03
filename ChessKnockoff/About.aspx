<%@ Page Title="About" Language="C#" MasterPageFile="~/BaseWithHeaderNav.master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ChessKnockoff.WebForm9" %>

<asp:Content ContentPlaceHolderID="BaseContentWithHeaderNav" runat="server">
    <script type="text/javascript">
        //Contains all the code for the board
        function init() {
            //Create the configuration variable for the chessboard
            var cfg = {
                //Disable dragging of pieces
                draggable: false,
                position: "nnnnknnn/pppppppp/2p2p2/1pppppp1/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1",
            };

            //Create the actualy HTML board in the div with the id, board
            board = ChessBoard('board', cfg);
        };

        //Call init once the DOM fully loads
        $(document).ready(init);
    </script>
    <div class="container mx-auto">
        <div class="text-center">
            <h2 class="mb-2 mt-4">About</h2>
        </div>
        <div class="row justify-content-center">
            <div id="board" style="width: 400px">
            </div>
        </div>
        <div class="row mt-2 mb-1 text-center justify-content-center">
            Just a regular game of chess. Same pieces, same rules, but different strategy.<br />
            Fine. It's just chess with the starting pieces arranged differently.<br /><br />
            
            But now with an ELO ranking system so you get paired up with people of roughly similar skill level!<br />
            No more getting smashed. No more whining. No more salt. Kappa.
        </div>
    </div>
</asp:Content>
