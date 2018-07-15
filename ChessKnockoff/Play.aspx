<%@ Page Title="Play" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="ChessKnockoff.WebForm9" %>

<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLogin" runat="server">
    <script type="text/javascript">
        //Contains all the code to be execute once the page has loaded
        function init() {
            //Create chess engine that validates move
            var game = new Chess("nnnnknnn/pppppppp/2p2p2/1pppppp1/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1");
            var statusEl = $('#status');

            // do not pick up pieces if the game is over
            // only pick up pieces for the side to move
            var onDragStart = function (source, piece, position, orientation) {
                if (game.game_over() === true ||
                    (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
                    (game.turn() === 'b' && piece.search(/^w/) !== -1)) {
                    return false;
                }
            };

            var onDrop = function (source, target) {
                // see if the move is legal
                var move = game.move({
                    from: source,
                    to: target,
                    promotion: 'q' // NOTE: always promote to a queen for example simplicity
                });

                // illegal move
                if (move === null) return 'snapback';

                updateStatus();
            };

            // update the board position after the piece snap 
            // for castling, en passant, pawn promotion
            var onSnapEnd = function () {
                board.position(game.fen());
            };

            var updateStatus = function () {
                var status = '';

                var moveColor = 'White';
                if (game.turn() === 'b') {
                    moveColor = 'Black';
                }

                // checkmate?
                if (game.in_checkmate() === true) {
                    status = 'Game over, ' + moveColor + ' is in checkmate.';
                }

                // draw?
                else if (game.in_draw() === true) {
                    status = 'Game over, drawn position';
                }

                // game still on
                else {
                    status = moveColor + ' to move';

                    // check?
                    if (game.in_check() === true) {
                        status += ', ' + moveColor + ' is in check';
                    }
                }

                statusEl.html(status);
            };

            //Create the actualy HTML board in the div with the id, board
            var board = ChessBoard('board');

            //Create the SignaIR connection to the server
            var contosoChatHubProxy = $.connection.contosoChatHub;

            //Any functions that the server can call
            contosoChatHubProxy.client.updatePosition = function (fenString) {
                //Set the board fen string
                board.position("fenString");
            };

            updateStatus();
        };

        //Call init once the DOM fully loads
        $(document).ready(init);
    </script>
    <div class="inputForm mx-auto">
        <div class="text-center">
            <img class="mb-4 mt-4" src="/logo.png" width="72" height="72">
            <h2 class="mb-2">Play</h2>
        </div>
    </div>
    <div class="container mt-2">
        <div class="row justify-content-center" style="margin: 0 auto;">
            <h3 id="hedOpponentName"></h3>
        </div>
        <div class="row mt-1 justify-content-center">
            <div class="wrap">
                <div id="board" style="width: 500px">
                </div>
            </div>
        </div>
        <div class="form-group">
            <button id="btnSubmitLogin" class="btn btn-lg btn-primary" type="submit"  data-toggle="button" aria-pressed="false"">Find game</button>
        </div>
    </div>
</asp:Content>
