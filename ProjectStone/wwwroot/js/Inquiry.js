// For DataTables
var dataTable;

$(document).ready(function() {
    // Call the Api Method made in the Controller. 
    loadDataTable("GetInquiryList");
});

function loadDataTable(url) {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/inquiry/" + url
        },
        "columns": [
            { "data": "id", "width": "10%" },
            { "data": "fullName", "width": "15%" }, // data columns MUST be in camelCase in order to work.
            { "data": "phoneNumber", "width": "10%" },
            { "data": "email", "width": "10%" },
            {
                "data": "id",
                "render": function(data) {
                    // data = id of inquiry.
                    return `
                            <div class="text-center">
                               <a href="/Inquiry/Details/${data}" class="btn btn-success text-white" style="cursor:pointer;">
                                   <i class="fas fa-edit"></i>
                               </a>
                            </div>
                            `;
                },
                "width": "5%"
            }
        ]
    });
}