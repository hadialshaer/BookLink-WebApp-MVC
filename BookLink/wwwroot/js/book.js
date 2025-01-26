
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/book/getall' },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'listPrice', "width": "15%" },
            { data: 'author', "width": "20%" },
            { data: 'bookCategory.categoryName', "width": "15%" }
            //{ data: 'Actions', "width": "25%" }
        ]
    });

}