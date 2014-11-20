'use strict';

$(function () {
    $(function () {
        var hub = $.connection.notificationHub;
        hub.client.broadcast = function (name, message) {
            if ($('#logPanel').css('display') == 'none') {
                $('#logPanel').show('slow');
                $('#logPanel').append('Project ' + name + 'is being deployed.');
            }

            console.log(name + ': ' + message);
            $('#logPanel').append(message);
        }

        $.connection.hub.logging = true;
        $.connection.hub.start();
    });

    var button = $('#submitButton');

    if ($('#ProjectId').val() == '') {
        button.attr('disabled', 'disabled');
        $('#logPanel').hide();
    } else {
        button.removeAttr("disabled");
        $('#logPanel').hide();
    }

    $(document).on('submit', '#requestForm', function (e) {
        disableControls();
        $.ajax({
            url: $(this).attr('action'),
            type: $(this).attr('method'),
            data: $(this).serialize(),
            dataType: 'text json',
            cache: false,
            success: function (message) {
                enableControls();
            },
            error: function (message) {
                console.log('error');
                enableControls();
            }
        });

        e.preventDefault();
    });

    var disableControls = function () {
        $(button).attr('disabled', 'disabled');
        $(button).text('')
            .append($('<li class="fa fa-refresh fa-spin"></li>'))
            .append(' Deploying...');


        $('#ProjectId').attr('disabled', 'disabled');
    }

    var enableControls = function () {
        $(button).removeAttr("disabled");
        $(button).text("Deploy this build");
        $('#ProjectId').removeAttr("disabled");
    }
});