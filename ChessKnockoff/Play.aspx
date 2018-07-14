<%@ Page Title="Play" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="ChessKnockoff.WebForm9" %>

<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLogin" runat="server">
    <script type="text/javascript">
        //Contains all the code for the board
        function init() {
            //Initialise the chess engine to only allow legal moves
            var board;

            //Create chess engine that validates move with the custom start
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
                //fenEl.html(game.fen());
                //pgnEl.html(game.pgn());
            };

            //Create the configuration variable for the chessboard
            var cfg = {
                draggable: true,
                position: "nnnnknnn/pppppppp/2p2p2/1pppppp1/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1",
                onDragStart: onDragStart,
                onDrop: onDrop,
                onSnapEnd: onSnapEnd
            };

            //Create the actualy HTML board in the div with the id, board
            board = ChessBoard('board', cfg);

            updateStatus();
        };

        //Call init once the DOM fully loads
        $(document).ready(init);
    </script>
    <div class="container mt-2 mx-auto">
        <div class="row justify-content-center" style="margin: 0 auto;">
            <h3 id="hedOpponentName" runat="server"></h3>
        </div>
        <div class="row mt-1 justify-content-center">
            <div id="board" style="width: 550px">
            </div>
        </div>
    </div>
</asp:Content>
