$('.deleteTask').click(function () {
    $('#myDelete').attr('action', '/Task/Delete/' + this.id);
});
    