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

            //Make an empty board
            var board = ChessBoard('board', "");

            //The start button
            var btnPlay = $("#btnPlay");

            //The fail alert
            var altFail = $("#altFail");

            //The opponent's username heading element
            var hedOpponent = $("#hedOpponentName");

            //Hide the game start button
            btnPlay.hide();

            //Hide the error emssage
            altFail.hide();

            //Create chess engine that validates move
            //var game = new Chess("nnnnknnn/pppppppp/2p2p2/1pppppp1/8/8/PPPPPPPP/RNBQKBNR w KQ - 0 1");
            //var statusEl = $('#status');

            //Create the SignaIR connection to the server
            var gameHubProxy = $.connection.gameHub;

            //Function to setup the game
            gameHubProxy.client.start = function (fenString, opponentUsername, side) {
                console.log(fenString + " " + opponentUsername + " " + side);

                //Hide the search for game button
                btnPlay.hide();

                //Display the opponent's username
                hedOpponent.html(opponentUsername);

                //Configure the board
                var cfg = {
                    draggable: true,
                    position: fenString,
                    orientation: side,
                    pieceTheme: 'Content/Images/{piece}.png',
                };

                //Create the actualy HTML board in the div with the id, board
                board = ChessBoard('board', cfg);
            }

            //Any functions that the server can call

            //Update the board
            gameHubProxy.client.updatePosition = function (fenString) {
                //Set the board fen string
                board.position(fenString);
            };

            //Display game over
            gameHubProxy.client.gameOver = function (fenString) {
                
            };

            //Display win
            gameHubProxy.client.gameWon = function (fenString) {

            };

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
                }
            });

            //When the connection is disconnected for any reason
            $.connection.hub.disconnected(function () {
                setTimeout(function () {
                    $.connection.hub.start();
                }, 5000); //Try to restart the connectiona after 5 seconds
            });

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

            function onDrop(source, target) {
                gameHubProxy.server.makeTurn(source, target);
            }
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
                    Sorry but a connection to the game server could not be created. Please try again at a later time.
                </div>
                <button id="btnPlay" class="btn btn-lg btn-primary" type="submit" data-toggle="button" aria-pressed="false"">Find game</button>
            </div>
        </div>
    </div>
</asp:Content>
