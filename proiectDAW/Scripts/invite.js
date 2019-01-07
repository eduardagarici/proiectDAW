$('.acceptButton').click(function () {
    $('#myForm1').attr('action', '/Invite/Accept/' + this.id);
});

$('.declineButton').click(function () {
    $('#myForm2').attr('action', '/Invite/Decline/'  + this.id);
});      