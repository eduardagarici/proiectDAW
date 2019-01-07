$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

$('.deleteButton').click(function () {
    $('#myForm').attr('action', '/Users/Delete/' + this.id);
});        