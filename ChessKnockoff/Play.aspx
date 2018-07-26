<%@ Page Title="Play" Language="C#" MasterPageFile="~/BaseWithHeaderNavLogin.master" AutoEventWireup="true" CodeBehind="Play.aspx.cs" Inherits="ChessKnockoff.PlayForm" %>
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
                pieceTheme: 'Content/Pieces/{piece}.png'
            };
            var board = ChessBoard("board", cfg);
            var randomMoveTimer;
            var game;

            //Holds whether confetti should be made
            var showEffect = false;

            //Store play information
            var gameData = {
                orientation: "",
                currentTurn: "white",
                opponentUsername: "",
                lookingForGame: false
            }

            //The wrapper
            var divWrapper = ("#wrapper");

            //The afk win and lose alerts
            var altAfkLose = $("#altAfkLose");
            var altAfkWin = $("#altAfkWin");

            //The draw alter
            var altDraw = $("#altDraw");

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

            //The already playing alert
            var altAlreadyPlaying = $("#altAlreadyPlaying");

            //Hide the game start button
            btnPlay.hide();

            //Function to hide messages/alerts
            var hideAllAlert = function () {
                //Hide the alerts/messages until needed
                altFail.hide();
                altLeave.hide();
                altLose.hide();
                altWin.hide();
                msgTurn.hide();
                altDraw.hide();
                altAfkLose.hide()
                altAfkWin.hide();
                altAlreadyPlaying.hide();
            }

            //Hide all alerts
            hideAllAlert();

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
                gameHubProxy.server.makeTurn(source, target);
            }

            //Method to set the board but not allowing dragging of pieces
            var setBoard = function fenString(fenString, draggable) {
                board.destroy();
                var cfg = {
                    position: fenString,
                    draggable: draggable,
                    pieceTheme: 'Content/Pieces/{piece}.png',
                    orientation: gameData.orientation,
                    onDragStart: onDragStart,
                    onDrop: onDrop
                }
                //Create the board in the DOM with the configuration
                board = ChessBoard("board", cfg);
            }

            var resetView = function (fenString, showPlay, draggable) {
                //Stop any particles effects from being made
                showEffect = false;

                //Remove all messages/alerts
                hideAllAlert();

                //Show the board
                setBoard(fenString, draggable);

                //Show the button
                btnPlay.show();

                //Stop the timer if there is still a timer
                clearTimeout(randomMoveTimer);

                //Reset the button state
                if (showPlay) {
                    //If tru then show the play button
                    btnPlay.button('toggle');
                    btnPlay.html("Find game");
                } else {
                    //Else hide it
                    btnPlay.hide();
                }

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

            //Create the SignaIR connection to the server
            var gameHubProxy = $.connection.gameHub;

            //Any functions that the server can call

            //Function to setup the game
            gameHubProxy.client.start = function (fenString, opponentUsername, side) {
                resetView(fenString, false, true);

                //Display the opponent's username
                hedTitle.html(opponentUsername);

                //Turn off the loading visual
                clearTimeout(randomMoveTimer);

                //Store the side
                gameData.orientation = side;
                //White always goes first
                gameData.currentTurn = "white";
                //Store the username
                gameData.opponentUsername = opponentUsername;

                resetView(fenString, false, true);

                //Show the message
                msgTurn.show();

                //Show whose turn it is
                showTurn();
            }

            //Call if the player is already playing
            gameHubProxy.client.alreadyPlaying = function () {
                //Make the board empty
                resetView("", true, false);

                //Show that the user is already playing
                altAlreadyPlaying.show();
            }

            gameHubProxy.client.afkWin = function () {
                //Lock the current view
                resetView(board.fen(), true, false);

                //Show the lose alert
                altAfkWin.show();
            }

            //Call if the player needs to make a move
            gameHubProxy.client.afkLose = function () {
                //Lock the current board in its state
                resetView(board.fen(), true, false);

                //Show the win alert
                altAfkLose.show();
            }

            //Call to warn the player to make a turn soon
            gameHubProxy.client.afkWarning = function () {
                //Show the warning
                msgTurn.html("<span class='text-danger'>Make a move or it is an automatic forfeit.</span>");
            }

            //Shows that the player is in queue
            gameHubProxy.client.inQueue = function () {
                //The game is actively searching

                //Make sure to hide all alerts just in case
                hideAllAlert();

                //Show that the matchmaking has started
                btnPlay.html("Looking for game...");

                //Create a board in the actual chess position
                setBoard("start", false);

                //Intialise the loading chess engine
                game = new Chess();

                //Make the chess board make random moves after a short delay
                randomMoveTimer = window.setTimeout(makeRandomMove, 500);
            }

            //Update the board
            gameHubProxy.client.updatePosition = function (fenString, turn) {
                //Set the board fen string
                board.position(fenString);

                //Set the persons current turn text only if their turn has changed
                if (gameData.currentTurn !== turn) {
                    gameData.currentTurn = turn;
                    showTurn();
                }
            };

            //The game drawed
            gameHubProxy.client.gameDraw = function () {
                //Retain the board state
                resetView(board.fen(), true, false);
                //Show the proper message
                altDraw.show();
            }

            //Opponent has left
            gameHubProxy.client.opponentLeft = function () {
                resetView(board.fen(), true, false);
                //Show the proper message
                altLeave.show();
            }

            //Display whether they lost or won
            gameHubProxy.client.gameFinish = function (winner) {
                //Clear the view
                resetView(board.fen(), true, false);

                //This player won
                if (gameData.orientation === winner) {
                    //Show the win alert
                    altWin.show();
                    //Create some conffeti
                    createConffeti();
                } else {
                    //Show lose message
                    altLose.show();
                    //Create some raindrops
                    createRain();
                }
            };

            //Call to create rain effect
            function createRain() {
                //Allow confetti to respawn
                showEffect = true;

                //Make 250 drops
                for (var i = 0; i < 100; i++) {
                    create(i);
                }

                //Create the rain elements 
                function create(i) {
                    //Create the actual rain in the div of wrapper
                    $('<div class="drop" id="drop' + i + '"></div>').css("left", "323px").css("top", "0px").appendTo('.wrapper');

                    //Make them drop
                    drop(i);
                }

                //Make them fall
                function drop(x) {
                    $('.drop' + x).animate({
                        top: "100%",
                    }, 1000, function () {
                        reset(x);
                    });
                }

                //Once they have fallen reset and put them at the top and into random positions
                function reset(x) {
                    $('.drop' + x).animate({
                        "top": "0px",
                        "left": "300px"
                    }, 0, function () {
                        //Check if they should be reset
                        if (showEffect) {
                            //If they should allow them to drop
                            drop(x);
                        } else {
                            //Remove them if they are not needed
                            $('.drop' + x).remove();
                        }
                    });
                }
            }

            //Call to create confetti
            function createConffeti() {
                //Allow confetti to respawn
                showEffect = true;

                //Make 250 pieces
                for (var i = 0; i < 100; i++) {
                    create(i);
                }

                //Create the confetti elements 
                function create(i) {
                    //Randomly assign their properties
                    var width = Math.random() * 8;
                    var height = width * 0.4;
                    var colourIdx = Math.ceil(Math.random() * 3);
                    var colour = "red";
                    switch (colourIdx) {
                        case 1:
                            colour = "yellow";
                            break;
                        case 2:
                            colour = "blue";
                            break;
                        default:
                            colour = "red";
                    }

                    //Create the actual confetti in the div of wrapper
                    ($('<div class="confetti-' + i + ' ' + colour + '"></div>').css({
                        "width": width + "px",
                        "height": height + "px",
                        "top": -Math.random() * 20 + "%",
                        "left": Math.random() * 100 + "%",
                        "opacity": Math.random() + 0.5,
                        "transform": "rotate(" + Math.random() * 360 + "deg)"
                    }).appendTo('.wrapper'));
                    //Wrapper takes up the full screen and has overflow set to hide

                    drop(i);
                }

                //Make them fall at different speeds and to different positions
                function drop(x) {
                    $('.confetti-' + x).animate({
                        top: "100%",
                        left: "+=" + Math.random() * 15 + "%"
                    }, Math.random() * 3000 + 3000, function () {
                        reset(x);
                    });
                }

                //Once they have fallen reset and put them at the top
                function reset(x) {
                    $('.confetti-' + x).animate({
                        "top": -Math.random() * 20 + "%",
                        "left": "-=" + Math.random() * 15 + "%"
                    }, 0, function () {
                        //Check if they should be reset
                        if (showEffect) {
                            //If they should allow them to drop
                            drop(x);
                        } else {
                            //Remove them if they are not needed
                            $('.confetti-' + x).remove();
                        }
                    });
                }
            }

            // Open a connection to the server hub
            $.connection.hub.logging = true; // Enable client side logging

            //When the start search button is pressed
            btnPlay.click(function () {
                //Store the new state of the button
                var isPressed = btnPlay.attr("aria-pressed") == "false";

                //Depending on whether it has been pressed start or end matchmaking
                if (isPressed) {
                    //Change the value of the button
                    btnPlay.html("Looking for game...");

                    //Stop making particles
                    showEffect = false;

                    //Call the server function to match make
                    gameHubProxy.server.findGame();
                } else {
                    //Change the text of the button
                    btnPlay.html("Find game");

                    //Call the function to stop match make
                    gameHubProxy.server.quitFindGame();

                    //Stop the find game visual
                    setBoard("", false);

                    //Stop the chessboard from making moves
                    clearTimeout(randomMoveTimer);
                    //Call the function to stop match make
                    gameHubProxy.server.quitFindGame();
                }
            });

            //When the connection is disconnected for any reason
            $.connection.hub.disconnected(function () {
                //Reset the view
                resetView("", false, false);

                //Show the error message
                altFail.show();
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
    <div class="wrapper">
    </div>
    <div class="container mt-2">
        <div class="row mt-1 mb-1 justify-content-center">
            <div id="board" style="width: 400px">
            </div>
        </div>
        <div class="row justify-content-center mt-1" >
            <div id="msgTurn" style="width: 400px"></div>
        </div>
        <div class="row mt-2 justify-content-center">
            <div class="text-center" style="width: 400px">
                <div id="altAlreadyPlaying" class="alert alert-info" role="alert">
                    Please stop looking for a game or finish the other game on the other machine to play.
                </div>
                <div id="altFail" class="alert alert-danger" role="alert">
                    Sorry but the connection to the game server broke. Please try again at a later time.
                </div>
                <div id="altWin" class="alert alert-success" role="alert">
                    Wow. You won...
                </div>
                <div id="altDraw" class="alert alert-secondary" role="alert">
                    It's a draw, both of you both suck...
                </div>
                <div id="altLose" class="alert alert-warning" role="alert">
                    Not surprising, you lost...
                </div>
                <div id="altAfkLose" class="alert alert-warning" role="alert">
                    You took too long to make a move...
                </div>
                <div id="altAfkWin" class="alert alert-warning" role="alert">
                    Your opponent got bored and went afk. Here is a free win...
                </div>
                <div id="altLeave" class="alert alert-info" role="alert">
                    The opponent has disconnected. Not surprising, no one wants to play with you.
                </div>
            </div>
        </div>
        <div class="row justify-content-center mb-2">
            <button id="btnPlay" class="btn btn-lg btn-primary" type="submit" data-toggle="button" aria-pressed="false"">Find game</button>
        </div>
    </div>
</asp:Content>
