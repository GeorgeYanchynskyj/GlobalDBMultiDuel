var lastUpdate = 0;
var distance = 24;
var score = [10, 10];
var Pnum;
var playing = true;
var inited = false;
var offline = false;
var state;
var GameID;
var ticktime = 100;

//var chat = $.connection.chatHub;


function tick() {

    if (playing) {
        var chat = $.connection.chatHub;
        chat.server.tick(GameID);
        setTimeout(tick, ticktime);
    }
    
}

$(document).ready(function () {
    lastUpdate = 0;

    //var chat = $.connection.chatHub;
    //chat.server.playerTest(0, player.getX(), player.getY(), player.getHeight(), player.getWidth());
    //chat.server.playerTest(1, opponent.getX(), opponent.getY(), opponent.getHeight(), opponent.getWidth());
    //chat.server.addBox(box.getX(), box.getY(), box.getHeight(), box.getWidth());  
    var chat = $.connection.chatHub;

    $('#top').bind("pointerdown", function () { chat.server.moove(1, Pnum, GameID); });
    $('#bot').bind("pointerdown", function () { chat.server.moove(2, Pnum, GameID); });
    $('#left').bind("pointerdown", function () { chat.server.moove(4, Pnum, GameID); });
    $('#right').bind("pointerdown", function () { chat.server.moove(3, Pnum, GameID); });
   
    $('#topleft').bind("pointerdown", function () { chat.server.moove(1, Pnum, GameID); chat.server.moove(4, Pnum, GameID); });
    $('#topright').bind("pointerdown", function () { chat.server.moove(1, Pnum, GameID); chat.server.moove(3, Pnum, GameID); });
    $('#botleft').bind("pointerdown", function () { chat.server.moove(2, Pnum, GameID); chat.server.moove(4, Pnum, GameID); });
    $('#botright').bind("pointerdown", function () { chat.server.moove(2, Pnum, GameID); chat.server.moove(3, Pnum, GameID); });

    $('#top').bind("pointerup", function () { chat.server.stop(1, Pnum, GameID); });
    $('#bot').bind("pointerup", function () { chat.server.stop(2, Pnum, GameID); });
    $('#left').bind("pointerup", function () { chat.server.stop(4, Pnum, GameID); });
    $('#right').bind("pointerup", function () { chat.server.stop(3, Pnum, GameID); });

    $('#topleft').bind("pointerup", function () { chat.server.stop(1, Pnum, GameID); chat.server.stop(4, Pnum, GameID); });
    $('#topright').bind("pointerup", function () { chat.server.stop(1, Pnum, GameID); chat.server.stop(3, Pnum, GameID); });
    $('#botleft').bind("pointerup", function () { chat.server.stop(2, Pnum, GameID); chat.server.stop(4, Pnum, GameID); });
    $('#botright').bind("pointerup", function () { chat.server.stop(2, Pnum, GameID); chat.server.stop(3, Pnum, GameID); });

    $('#fire').bind("pointerdown", function () { chat.server.moove(0, Pnum, GameID); });
    $('#fire').bind("pointerup", function () { chat.server.stop(0, Pnum, GameID); });

    //requestAnimationFrame(update);
});

$(document).keydown(function (event) {
    var event = event || window.event;
    
    var chat = $.connection.chatHub;

    //chat.client.broadcastMessage('Upper:', String.fromCharCode(event.keyCode).toUpperCase());
    //chat.client.broadcastMessage('Lower:', String.fromCharCode(event.keyCode));

    if (String.fromCharCode(event.keyCode).toUpperCase() == 'W') chat.server.moove(1, Pnum, GameID);
    else if (String.fromCharCode(event.keyCode).toUpperCase() == 'S') chat.server.moove(2, Pnum, GameID);
    if (String.fromCharCode(event.keyCode).toUpperCase() == 'D') chat.server.moove(3, Pnum, GameID);
    else if (String.fromCharCode(event.keyCode).toUpperCase() == 'A') chat.server.moove(4, Pnum, GameID);
    if (String.fromCharCode(event.keyCode).toUpperCase() == ' ') chat.server.moove(0, Pnum, GameID);
    if (offline) {
        if (String.fromCharCode(event.keyCode).toUpperCase() == '&') chat.server.moove(1, 1, GameID);
        else if (String.fromCharCode(event.keyCode).toUpperCase() == '(') chat.server.moove(2, 1, GameID);
        if (String.fromCharCode(event.keyCode).toUpperCase() == "'") chat.server.moove(3, 1, GameID);
        else if (String.fromCharCode(event.keyCode).toUpperCase() == '%') chat.server.moove(4, 1, GameID);
        if (String.fromCharCode(event.keyCode).toUpperCase() == 'K') chat.server.moove(0, 1, GameID);
    }

    if (String.fromCharCode(event.keyCode).toUpperCase() == 'O') chat.server.save(GameID);

    if (state == 1) chat.server.joinGame(String.fromCharCode(event.keyCode) - 0);
    if (state == 4) chat.server.loadGame(String.fromCharCode(event.keyCode) - 0);

    if (String.fromCharCode(event.keyCode).toUpperCase() == 'P'){
        if (playing) playing = false;
        else if (inited) {
            playing = true;
            tick();
        }
    }

    return false;
});

$(document).keyup(function (event) {
    var event = event || window.event;

    var chat = $.connection.chatHub;

    if (String.fromCharCode(event.keyCode).toUpperCase() == 'W') chat.server.stop(1, Pnum, GameID);
    if (String.fromCharCode(event.keyCode).toUpperCase() == 'S') chat.server.stop(2, Pnum, GameID);
    if (String.fromCharCode(event.keyCode).toUpperCase() == 'D') chat.server.stop(3, Pnum, GameID);
    if (String.fromCharCode(event.keyCode).toUpperCase() == 'A') chat.server.stop(4, Pnum, GameID);
    if (String.fromCharCode(event.keyCode).toUpperCase() == ' ') chat.server.stop(0, Pnum, GameID);
    if (offline) {
        if (String.fromCharCode(event.keyCode).toUpperCase() == '&') chat.server.stop(1, 1, GameID);
        if (String.fromCharCode(event.keyCode).toUpperCase() == '(') chat.server.stop(2, 1, GameID);
        if (String.fromCharCode(event.keyCode).toUpperCase() == "'") chat.server.stop(3, 1, GameID);
        if (String.fromCharCode(event.keyCode).toUpperCase() == '%') chat.server.stop(4, 1, GameID);
        if (String.fromCharCode(event.keyCode).toUpperCase() == 'K') chat.server.stop(0, 1, GameID);
    }

    return false;
});

