function FillComboBox(ActionName, Target) {
    $.ajax({
        type: "GET",
        url: "" + ActionName + "",
        dataType: "json",
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                $.each(response.list, function () {
                    
                    $('#' + Target + '').append($("<option/>").val(this.id).text(this.name));
                });
            }
            else {
                $("#textError").text(response.responseText);
                $("#ErrorModal").modal('show');
            }
        },
        error: function (response) {
            alert("Error");
        }
    });
}