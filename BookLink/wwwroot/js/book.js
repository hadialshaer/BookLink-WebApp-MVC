$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/book/getall' }, // Fetches data from your API endpoint
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'listPrice', "width": "15%" },
            { data: 'author', "width": "20%" },
            { data: 'bookCategory.categoryName', "width": "15%" },
            {
                data: 'bookId', // The unique ID for each book
                "render": function (data) {
                    return `
                        <div class="d-flex justify-content-center">
                            <a href="/Admin/Book/Upsert?id=${data}" class="btn btn-primary"">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <a href="/Admin/Book/Delete/${data}" class="btn btn btn-danger">
                                <i class="bi bi-trash"></i> Delete
                            </button>
                        </div>`
                },
                "width": "25%"
            }
        ]
    });
}
