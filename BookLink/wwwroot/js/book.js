var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/book/getall' }, // Fetches data from your API endpoint
        "columns": [
            { data: 'title', "width": "25%", "className": "text-center" },
            { data: 'listPrice', "width": "15%", "className": "text-center" },
            { data: 'author', "width": "20%", "className": "text-center" },
            { data: 'bookCategory.categoryName', "width": "15%", "className": "text-center" },
            {
                data: 'bookId', // The unique ID for each book
                "render": function (data) {
                    return `
                        <div class="d-flex justify-content-center">
                            <a href="/Admin/Book/Upsert?id=${data}" class="btn btn-primary">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button onClick=Delete("/Admin/Book/Delete/${data}") class="btn btn btn-danger mx-1">
                                <i class="bi bi-trash"></i> Delete
                            </button>
                        </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}
