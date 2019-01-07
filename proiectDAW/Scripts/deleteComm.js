$('.deleteComm').click(function () {
    $('#myDelete').attr('action', '/Comment/Delete/' + this.id);
});
