$('#delete').click(function (handler) {
    if (confirm("Do you really want to delete this entry and all linked data?")) {
        $.ajax({
            url: '/' + Controller + '/Delete/' + Pk,
            async: true,
            dataType: 'json',
            type: 'post',
            success: function () {
                alert('Deletetion was successfull.\nYou will be redirected.');
                window.location.href = '/' + Controller;
            },
            error: function () {
                displayErrorInfo("An error occoured while trying to delete entry!<br>Please try again later.");
            }
        });
    }
});
$('#edit').click(function (handler) {
    window.location.href = '/' + Controller + '/Edit/' + Pk;
});