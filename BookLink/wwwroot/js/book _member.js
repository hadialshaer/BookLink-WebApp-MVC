var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/member/book/getall' },
        "columns": [
            {
                data: 'title',
                "width": "20%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'author',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'category.categoryName',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'transactionType',
                "render": function (data) {
                    let badgeClass = data === "Lend" ? "bg-success" : "bg-info"; // Green for Lend, Blue for Sell
                    return `<span class="badge ${badgeClass}">${data || "N/A"}</span>`;
                },
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'listPrice',
                "render": function (data, type, row) {
                    return row.transactionType === "Sell" && data
                        ? `$${data.toFixed(2)}`
                        : "N/A";
                },
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'maxLendDurationDays',
                "render": function (data, type, row) {
                    return row.transactionType === "Lend" && data
                        ? `${data} days`
                        : "N/A";
                },
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'dueDate',
                "render": function (data, type, row) {
                    return row.transactionType === "Lend" && data
                        ? `<span class="badge bg-warning">${data}</span>`
                        : "N/A";
                },
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'bookId',
                "render": function (data) {
                    return `
                        <div class="d-flex justify-content-center">
                            <a href="/Member/Book/Upsert?id=${data}" class="btn btn-primary btn-sm">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button onClick=Delete("/Member/Book/Delete/${data}") 
                                    class="btn btn-danger btn-sm mx-1">
                                <i class="bi bi-trash"></i> Delete
                            </button>
                        </div>`;
                },
                "width": "20%",
                "className": "text-center"
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
                    toastr.success(data.message || "Book deleted successfully.");
                },
                error: function () {
                    toastr.error("Failed to delete the book. Please try again.");
                }
            });
        }
    });
}