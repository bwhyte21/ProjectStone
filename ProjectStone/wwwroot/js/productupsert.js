$(document).ready(function() {
    $('.summernote').summernote({
        height: 250,
        toolbar: [
            // [groupName, [list of button]]
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['fontSize', ['fontsize']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']]
        ]
    });
});

function validateInput() {
    if (document.getElementById("uploadBox").value == "") {
        Swal.fire(
            'Error!',
            'Please upload an image!',
            'error'
        );
        return false;
    }
    return true;
}