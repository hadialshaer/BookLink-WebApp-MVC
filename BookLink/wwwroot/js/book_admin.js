var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/book/getall' },
        "columns": [
            { data: 'title', "width": "20%", "className": "text-center", "render": (data) => data || "N/A" },
            { data: 'author', "width": "15%", "className": "text-center", "render": (data) => data || "N/A" },
            { data: 'category', "width": "15%", "className": "text-center", "render": (data, type, row) => row.category?.categoryName || "N/A" },
            {
                data: 'transactionType',
                "render": (data) => `<span class="badge ${data === "Lend" ? "bg-success" : "bg-info"}">${data || "N/A"}</span>`,
                "width": "10%", "className": "text-center"
            },
            {
                data: 'listPrice',
                "render": (data, type, row) => row.transactionType === "Sell" && typeof data === "number" ? `$${data.toFixed(2)}` : "N/A",
                "width": "10%", "className": "text-center"
            },
            {
                data: 'maxLendDurationDays',
                "render": (data, type, row) => row.transactionType === "Lend" && data ? `${data} days` : "N/A",
                "width": "10%", "className": "text-center"
            },
            {
                data: 'dueDate',
                "render": (data, type, row) => row.transactionType === "Lend" && data ? `<span class="badge bg-warning">${data}</span>` : "N/A",
                "width": "10%", "className": "text-center"
            },
            {
                data: 'bookStatus',
                "render": (data) => `<span class="badge bg-${data === 'Available' ? 'success' : 'danger'}">${data || "N/A"}</span>`,
                "width": "10%", "className": "text-center"
            },
            {
                data: 'bookId',
                "render": (data) => data ? `
                    <div class="d-flex justify-content-center">
                        <a href="/Admin/Book/Upsert?id=${data}" class="btn btn-primary btn-sm">
                            <i class="bi bi-pencil-square"></i> Edit
                        </a>
                        <button onClick="Delete('/Admin/Book/Delete/${data}')" 
                                class="btn btn-danger btn-sm mx-1">
                            <i class="bi bi-trash"></i> Delete
                        </button>
                    </div>` : "N/A",
                "width": "20%", "className": "text-center"
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
                type: "DELETE"
            }).done(function (data) {
                dataTable.ajax.reload();
                toastr.success(data.message || "Book deleted successfully.");
            }).fail(function () {
                toastr.error("Failed to delete the book. Please try again.");
            });
        }
    });
}
