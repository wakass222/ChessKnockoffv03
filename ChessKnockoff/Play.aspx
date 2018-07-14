<%@ Page Title="Play" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="ChessKnockoff.WebForm9" %>
<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLogin" runat="server">
    <script type="text/javascript">
        //Contains all the code for the board
        var init = function () {
            var statusEl = $('#status');
            var fenEl = $('#fen');
            var pgnEl = $('#pgn');

            // do not pick up pieces if the game is over
            // only pick up pieces for the side to move
            function onDragStart (source, piece, position, orientation) {
              if (game.game_over() === true ||
                  (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
                  (game.turn() === 'b' && piece.search(/^w/) !== -1)) {
                return false;
              }
            };

            function onDrop (source, target) {
              // see if the move is legal
              var move = game.move({
                from: source,
                to: target,
                promotion: 'q' // NOTE: always promote to a queen for example simplicity
              });

                //If the move is illegal
                if (move === null) {
                    //Return the piece to the original position
                    return 'snapback';
                }

                //Check the game status
                updateStatus();
            };

            // update the board position after the piece snap 
            // for castling, en passant, pawn promotion
            function onSnapEnd () {
              board.position(game.fen());
            };

            //Check for game over and checks
            function updateStatus () {
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
              fenEl.html(game.fen());
              pgnEl.html(game.pgn());
            };

            //Create the configuration for the board
            var cfg = {
                //Allow the pieces to be dragged
                draggable: true,
                //Set it up 
                position: 'rnbqkbnr/pppppppp/8/1PP2PP1/PPPPPPPP/PPPPPPPP/PPPPPPPP/PPPPPPPP w kq - 0 1',
                //Set the events to their respective functions
                onDragStart: onDragStart,
                onDrop: onDrop,
                onSnapEnd: onSnapEnd
            };

            //Create the board with reference to the board ID
            var board = ChessBoard('board', cfg);
        };

        //Call init once the DOM fully loads
        $(document).ready(init);
    </script>
    <div id="board" style="width: 400px">
    </div>
</asp:Content>
