 $('.deleteButton').click(function () {
    $('#myForm').attr('action', '/Project/Delete/' + this.id);
});        

$('.deleteButton2').click(function () {
    $('#myForm2').attr('action', '/Project/' + $(this).data('project') +'/DeleteMember/' + this.id);
});      