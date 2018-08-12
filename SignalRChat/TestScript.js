$(function () {

    // Declare a proxy to reference the hub.
    var chat = $.connection.chatHub;
    // Create a function that the hub can call to broadcast messages.
    //var event = $.connection.eventHub;

    var states = [["url(./images/p2.png)", "url(./images/P2onfire.png)"], ["url(./images/p1.png)", "url(./images/P1fire.png)"]];
    var boxStates = ["url(./images/Box.png)", "url(./images/BoxBrake1.png)", "url(./images/BoxBrake2.png)", "url(./images/BoxBrake3.png)", "url(./images/BoxBrake4.png)"];
    var players = ["player", "opponent"];

    chat.client.broadcastMessage = function (name, message) {
        // Html encode display name and message.
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();       
        // Add the message to the page.
        $('#discussion').append('<li><strong>' + encodedName
            + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
        //$('#discussion').text = encodedMsg;
    };
   
    chat.client.showGames = function (list) {
        if (state == 4) var m = ' Saved ';
        else var m = ' Open ';
        if (list != '') var encodedList = $('<div />').text(m + 'games: ' + list).html();
        else var encodedList = $('<div />').text('No' + m + 'games').html();

        $('#info').append('<li><strong>' + encodedList
            + '</strong>:&nbsp;&nbsp;' + '' + '</li>')
    }

    chat.client.poseObject = function (x, y, name) {

        $('#' + name).css('left', x + 'px');
        $('#' + name).css('top', y + 'px');
    }

    chat.client.poseObject = function (x, y, name) {

        $('#' + name).css('left', x + 'px');
        $('#' + name).css('top', y + 'px');
    }

    chat.client.setPlayer = function (num) {
        if (num == 0) {
            var p = '<div id=player class="sprite"></div>';
            $("body").append(p);          

            if (Pnum == 0 && state != 4) chat.server.addPlayer(0, parseInt($('#player').position().left), parseInt($('#player').position().top), $('#player').height(), $('#player').width(), "player", GameID);
        }      
        if (num == 1) {
            var p = '<div id=opponent class="sprite"></div>';
            $("body").append(p);

            if (Pnum == 0 && state != 4) chat.server.addPlayer(1, parseInt($('#opponent').position().left), parseInt($('#opponent').position().top), $('#opponent').height(), $('#opponent').width(), "opponent", GameID);
        }       
    }

    chat.client.getWalls = function () {
        for (i = 0; i < 4; i++) {
            var w = $('#wall' + i);
            chat.server.addWall(parseInt(w.position().left), parseInt(w.position().top), w.height(), w.width(), 'wall' + i, GameID);
        }
    }

    // events

    chat.client.setBoom = function (x, y, name) {

        var b = '<div id=' + name + ' class="boom"></div>';
        $("body").append(b);

        $('#' + name).css('left', x + 'px');
        $('#' + name).css('top', y + 'px');
    }

    chat.client.setBox = function (x, y, name) {

        var b = '<div id=' + name + ' class="box"></div>';
        $("body").append(b);

        $('#' + name).css('left', x + 'px');
        $('#' + name).css('top', y + 'px');
    }

    chat.client.setGhost = function (x, y, name) {

        var b = '<div id=' + name + ' class="ghost"></div>';
        $("body").append(b);

        $('#' + name).css('left', x + 'px');
        $('#' + name).css('top', y + 'px');
    }

    chat.client.setBonus = function (x, y, name) {

        var b = '<div id=' + name + ' class="bonus"></div>';
        $("body").append(b);

        $('#' + name).css('left', x + 'px');
        $('#' + name).css('top', y + 'px');
    }

    chat.client.setBullet = function (from, name) {

        var b = '<div id=' + name + ' class="bullet"></div>';
        $("body").append(b);

        if (from == 0)
        {
            $('#' + name).css('left', parseInt($('#player').position().left) + 'px');
            $('#' + name).css('top', parseInt($('#player').position().top) + 'px');
        }
        if (from == 1) {
            $('#' + name).css('left', parseInt($('#opponent').position().left) + 'px');
            $('#' + name).css('top', parseInt($('#opponent').position().top) + 'px');
        }
    }

    chat.client.gameOver = function (p) {
        if (p == 0) {
            $('#playerScore').text('Lose');
            $('#opponentScore').text('Win');
        }
        else {
            $('#playerScore').text('Win');
            $('#opponentScore').text('Lose');
        }
        playing = false;

        $('#save').remove();
        chat.server.gameOver(GameID);
    }

    chat.client.showHealth = function (p0, p1) {
        $('#playerScore').text(p0);
        $('#opponentScore').text(p1);
    }

    chat.client.animPlayer = function (p,s) {
        document.getElementById(players[p]).style.backgroundImage = states[p][s];
    }

    chat.client.animBox = function (name, state) {
        document.getElementById(name).style.backgroundImage = boxStates[state];
    }

    chat.client.removeObj = function (name) {
        $('#' + name).remove();
    }

    // setters

    chat.client.getSize = function () {
        chat.server.setSize(innerWidth, innerHeight, GameID);
    }

    chat.client.setNum = function (num1) {
        $('#displayname').val(num1);
        Pnum = num1;   
    }

    chat.client.setInit = function () {
        inited = true;
        setTimeout(tick, 500);
    }

    chat.client.setID = function (id) {
        GameID = id;

        $('#create').remove();
        $('#join').remove();
        $('#offline').remove();
        $('#load').remove();
        $('#save').css('top', 10 + '%');

        if (state == 0) str = 'Waiting for opponent...';
        if (state == 1) str = 'Connected';
        if (state == 3) str = 'Offline mode';
        if (state == 4) str = 'Loaded game';
        var encodedList = $('<div />').text(str).html();
        $('#info').append('<li><strong>' + encodedList
            + '</strong>:&nbsp;&nbsp;' + '' + '</li>')

        chat.client.broadcastMessage('Group:', GameID);
        
        if (state == 0) {
            chat.server.setSize(innerWidth, innerHeight, GameID);
            chat.client.getWalls();
        }
        if (state == 1) {
            Pnum = 1;
            state = 2;
        }
        if (state == 3) {
            
            offline = true;
            Pnum = 0;
        }
        if (state == 4) {
            state = 3;
            offline = true;
            Pnum = 0;
        }
    }

    chat.client.getWalls = function () {
        for (i = 0; i < 4; i++) {
            var w = $('#wall' + i);
            chat.server.addWall(parseInt(w.position().left), parseInt(w.position().top), w.height(), w.width(), 'wall' + i, GameID);
        }
    }

    
    $('#displayname').val(0);

    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            // Call the Send method on the hub.
            chat.server.send($('#displayname').val(), $('#message').val());
            
            // Clear text box and reset focus for next comment.
            $('#message').val('').focus();
        });

        $('#create').click(function () {
            chat.server.create();

            $('#displayname').val(0);
            Pnum = 0;

            state = 0;
        })
        $('#join').click(function () {
            chat.server.join();

            state = 1;
        })
        $('#offline').click(function () {

            state = 3;

            chat.server.offline();

        })
        $('#save').click(function () {
            chat.client.broadcastMessage('Safe:', 'game');
            chat.server.save(GameID);
            
        })
        $('#load').click(function () {
            state = 4;

            chat.client.broadcastMessage('Load:', 'game');
            chat.server.load();

        })
    });
});