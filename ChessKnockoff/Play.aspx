<%@ Page Title="Play" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="ChessKnockoff.WebForm9" %>
<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLoginTitle" runat="server">
    <script type="text/javascript">
        //Once the DOM has loaded set the title
        $(document).ready(function () {
            $("#title").html("Play");
        });
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="BaseContentHeaderNavLogin" runat="server">
    <script type="text/javascript">
        //Contains all the code to be execute once the page has loaded
        function init() {
            //Make global variables
            //Configure an empty board
            var cfg = {
                position: "",
                draggable: false,
                pieceTheme: 'Content/Images/{piece}.png'
            };
            var board = ChessBoard("board", cfg);
            var randomMoveTimer;
            var game;

            //Store play information
            var gameData = {
                orientation: "",
                currentTurn: "",
                opponentUsername: "",
                lookingForGame: false
            }

            //The turn message
            var msgTurn = $("#msgTurn");

            //The start button
            var btnPlay = $("#btnPlay");

            //The fail alert
            var altFail = $("#altFail");

            //The disconnect alert
            var altLeave = $("#altLeave");

            //The win alert
            var altWin = $("#altWin");

            //The lose alert
            var altLose = $("#altLose");

            //The title element
            var hedTitle = $("#title");

            //Hide the game start button
            btnPlay.hide();

            //Hide the alerts/messages until needed
            altFail.hide();
            altLeave.hide();
            altLose.hide();
            altWin.hide();
            msgTurn.hide();

            //Method to set the board
            var setBoard = function () {
                var cfg = {
                    position: "",
                    draggable: false,
                    pieceTheme: 'Content/Images/{piece}.png'
                }

                board = ChessBoard("board", cfg);
            }

            var resetView = function (fenString) {
                setBoard(fenString);
                //Show the button
                btnPlay.show();
                //Reset the button state
                btnPlay.attr("aria-pressed", "true");
                //Hide whose turn it is
                msgTurn.hide();
                //Reset the title
                hedTitle.html("Play");
            }

            //Show whoses turn it is
            var showTurn = function () {
                //Check which side the player is
                if (gameData.orientation == gameData.currentTurn) {
                    msgTurn.html("It is your turn");
                } else {
                    msgTurn.html("It is your opponent's turn");
                }
            }

            //Make random move function
            var makeRandomMove = function () {
                var possibleMoves = game.moves();

                // exit if the game is over
                if (game.game_over() === true ||
                    game.in_draw() === true ||
                    possibleMoves.length === 0) return;

                //Make a random move from array of possible moves
                var randomIndex = Math.floor(Math.random() * possibleMoves.length);
                game.move(possibleMoves[randomIndex]);
                //Set the board position to the newly generated move
                board.position(game.fen());
                
                //Make a new event with a 0.5 second delay
                randomMoveTimer = window.setTimeout(makeRandomMove, 500);
            };

            //Only allow dragging of owned pieces and must be their turn
            var onDragStart = function (source, piece, position, orientation) {
                //Check the board orientation and whether the piece is black or white owned
                if ((orientation === 'white' && piece.search(/^w/) === -1) ||
                    (orientation === 'black' && piece.search(/^b/) === -1)) {
                    return false;
                }

                //If it is not their turn then dont allow the piece to be dragged
                if (gameData.currentTurn !== orientation) {
                    return false;
                }
            };

            //Method call on how to process the board
            var onDrop = function (source, target, piece, newPos, oldPos, orientation) {
                //Sends the data to the server
                gameHubProxy.server.makeTurn(source, target).done(function () {
                    console.log("It finised");
                });
            }
            
            //Create the SignaIR connection to the server
            var gameHubProxy = $.connection.gameHub;

            //Function to setup the game
            gameHubProxy.client.start = function (fenString, opponentUsername, side) {
                //Hide the search for game button
                btnPlay.hide();

                //Display the opponent's username
                hedTitle.html(opponentUsername);

                //Turn off the look for a game
                clearTimeout(randomMoveTimer);

                //Store the side
                gameData.orientation = side;
                //White always goes first
                gameData.currentTurn = "white";
                //Store the username
                gameData.opponentUsername = opponentUsername;

                //Configure the board for actual playing
                var cfg = {
                    draggable: true,
                    position: fenString,
                    pieceTheme: 'Content/Images/{piece}.png',
                    onDrop: onDrop,
                    onDragStart: onDragStart
                };

                //Replace the current board
                board = ChessBoard('board', cfg);

                //Show the message
                msgTurn.show();

                //Show whose turn it is
                showTurn();

                //Change the orientation of the board
                board.orientation(side);
            }

            //Any functions that the server can call

            //Update the board
            gameHubProxy.client.updatePosition = function (fenString, turn) {
                //Set the board fen string
                board.position(fenString);

                //Set the persons current turn
                gameData.currentTurn = turn;
            };

            //Opponent has left
            gameHubProxy.client.opponentLeft = function () {
                resetView();
                //Show the proper message
                altLeave.show();
            }

            //Display game over
            gameHubProxy.client.gameOver = function () {
                //Clear the view
                resetView();
                //Show lose message
                altLose.show();
            };

            //Display win
            gameHubProxy.client.gameWon = function () {
                //Clear the view
                resetView();
                //Show the win message
                altWin.show();
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
                    
                    //Intialise the loading chess engine
                    game = new Chess();

                    //If the find game was successful then show the matchmaking visuals
                    //Make a frozen board in the default chess position
                    freezeBoard("start");

                    //Make the chess board make random moves after a delay
                    randomMoveTimer = window.setTimeout(makeRandomMove, 1000);

                    //Call the server function to match make
                    gameHubProxy.server.findGame().fail(function () {
                        clearTimeout(randomMoveTimer);
                    });
                } else {
                    btnPlay.html("Find game");

                    //Call the function to stop match make
                    gameHubProxy.server.quitFindGame();

                    //Stop the find game visual
                    setBoard("");

                    //Stop the chessboard from making moves
                    clearTimeout(randomMoveTimer);
                }
            });

            //When the connection is disconnected for any reason
            $.connection.hub.disconnected(function () {
                //Show the error message
                altFail.show();
                btnPlay.hide();

                //Clear the board
                setBoard("");
                //Stop the board from updating if they were searching
                clearTimeout(randomMoveTimer);
                /*
                setTimeout(function () {
                    $.connection.hub.start();
                }, 5000); //Try to restart the connectiona after 5 seconds
                */
            });

            //Start the connection to the hub
            $.connection.hub.start()
                .done(function () {
                    //Show the game play button
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
        <div class="row mt-1 justify-content-center">
            <div id="board" style="width: 400px">
            </div>
        </div>
        <div id="msgTurn" class="row mt-1 form-group text-left justify-content-center" style="width: 400px">
        </div>
        <div class="row mt-2 justify-content-center">
            <div class="form-group text-center" style="width: 400px">
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
        <div class="row justify-content-center mb-2">
            <button id="btnPlay" class="btn btn-lg btn-primary" type="submit" data-toggle="button" aria-pressed="false"">Find game</button>
        </div>
    </div>
</asp:Content>
