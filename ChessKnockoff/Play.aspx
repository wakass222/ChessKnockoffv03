<%@ Page Title="Play" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="ChessKnockoff.WebForm9" %>
<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLoginTitle" runat="server">
    Play
</asp:Content>

<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLogin" runat="server">
    <script type="text/javascript">
        //Contains all the code to be execute once the page has loaded
        function init() {
            //Code references board before it has been configured
            var board;

            //The start button
            var btnPlay = $("#btnPlay");

            //The fail alert
            var altFail = $("#altFail");

            //The disconnect alert
            var altLeave= $("#altLeave");

            //The win alert
            var altWin = $("#altWin");

            //The lose alert
            var altLose = $("#altLose");

            //The opponent's username heading element
            var hedOpponent = $("#hedOpponentName");

            //Hide the game start button
            btnPlay.hide();

            //Hide the alerts until needed
            altFail.hide();
            altLeave.hide();
            altLose.hide();
            altWin.hide();

            //Create chess engine that validates move
            //var game = new Chess("nnnnknnn/pppppppp/2p2p2/1pppppp1/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1");
            //var statusEl = $('#status');

            //Create the SignaIR connection to the server
            var gameHubProxy = $.connection.gameHub;

            //Function to setup the game
            gameHubProxy.client.start = function (fenString, opponentUsername, side) {
                //Hide the search for game button
                btnPlay.hide();

                //Display the opponent's username
                hedOpponent.html(opponentUsername);

                //Set up the board with pieces
                board.position(fenString);

                //Change the orientation of the board
                board.orientation(side);
            }

            //Any functions that the server can call

            //Update the board
            gameHubProxy.client.updatePosition = function (fenString) {
                //Set the board fen string
                console.log(fenString + " setting board");
                board.position(fenString);
            };

            //Display game over
            gameHubProxy.client.gameOver = function () {

            };

            //Display win
            gameHubProxy.client.gameWon = function () {

            };

            //Only allow dragging of owned pieces
            var onDragStart = function (source, piece, position, orientation) {
                if ((orientation === 'white' && piece.search(/^w/) === -1) ||
                    (orientation === 'black' && piece.search(/^b/) === -1)) {
                    return false;
                }
            };

            //Method call on how to process the board
            var onDrop = function (source, target, piece, newPos, oldPos, orientation) {
                gameHubProxy.server.makeTurn(source, target).done(function () {
                    console.log("It finised");
                });

                //Always return a snapback
                return "snapback";
            }

            // Open a connection to the server hub
            $.connection.hub.logging = true; // Enable client side logging

            //When the start search button is pressed
            btnPlay.click(function () {
                //Store the new state of the button
                var isPressed = btnPlay.attr("aria-pressed") == "false";

                //Depending on whether it has been pressed start or end matchmaking
                if (isPressed) {
                    //Show that the matchmaking has started
                    btnPlay.html("Looking for game...");

                    //Call the server function to match make
                    gameHubProxy.server.findGame();
                } else {
                    btnPlay.html("Find game");

                    //Call the function to stop match make
                    gameHubProxy.server.quitFindGame();
                }
            });

            //Make an empty board
            //Configure the board
            var cfg = {
                draggable: true,
                position: "",
                pieceTheme: 'Content/Images/{piece}.png',
                onDrop: onDrop,
                onDragStart: onDragStart
            };
            board = ChessBoard('board', cfg);

            //When the connection is disconnected for any reason
            $.connection.hub.disconnected(function () {
                //Show the error message
                altFail.show();
                btnPlay.hide();
                /*
                setTimeout(function () {
                    $.connection.hub.start();
                }, 5000); //Try to restart the connectiona after 5 seconds
                */
            });

            /*
            //Unhide the play game button
                btnPlay.show();
                //Click it to change the state to find game
                btnPlay.click();
                */

            //Start the connection to the hub
            $.connection.hub.start()
                .done(function () {
                    //Show the game
                    btnPlay.show();
                })
                .fail(function () {
                    //Show the error if a connection could not be made
                    altFail.show();
                });
        };

        //Call init once the DOM fully loads
        $(document).ready(init);
    </script>
    <div class="container mt-2">
        <div class="row justify-content-center">
            <h3 id="hedOpponentName"></h3>
        </div>
        <div class="row mt-1 justify-content-center">
            <div id="board" style="width: 500px">
            </div>
        </div>
        <div class="row mt-2 justify-content-center">
            <div class="form-group">
                <div id="altFail" class="alert alert-danger" role="alert">
                    Sorry but the connection to the game server broke. Please try again at a later time.
                </div>
                <div id="altWin" class="alert alert-success alert-dismissible fade show" role="alert">
                    Wow. You won...
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div id="altLose" class="alert alert-success alert-dismissible fade show" role="alert">
                    Not surprising, you lost...
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div id="altLeave" class="alert alert-warning alert-dismissible fade show" role="alert">
                    The opponent has disconnected. Congrats on freelo.
                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
            </div>
        </div>
        <div class="row justify-content-center">
            <button id="btnPlay" class="btn btn-lg btn-primary" type="submit" data-toggle="button" aria-pressed="false"">Find game</button>
        </div>
    </div>
</asp:Content>
