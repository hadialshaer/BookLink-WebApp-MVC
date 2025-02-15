var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#borrowTable').DataTable({
        "ajax": {
            url: '/member/borrow/getall',
            dataSrc: 'data',
            beforeSend: function () {
                $('#loadingSpinner').show();
            },
            complete: function () {
                $('#loadingSpinner').hide();
            },
            error: function (xhr) {
                $('#loadingSpinner').hide();
                $('#noDataMessage').show();
                const errorMessage = xhr.responseJSON?.error || xhr.statusText || 'Error loading data';
                toastr.error(errorMessage);
            }
        },
        "columns": [
            {
                data: 'bookTitle',
                "width": "15%",
                "className": "text-center"
            },
            {
                data: 'borrowerName',
                "width": "15%",
                "className": "text-center"
            },
            {
                data: 'phone',
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'location',
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'requestDate',
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'status',
                "render": function (data) {
                    const badgeClasses = {
                        'Pending': 'bg-warning',
                        'Approved': 'bg-success',
                        'Rejected': 'bg-danger',
                        'Returned': 'bg-info'
                    };
                    return `<span class="badge ${badgeClasses[data] || 'bg-secondary'}">${data}</span>`;
                },
                "width": "10%",
                "className": "text-center"
            },
            {
                data: 'dueDate',
                "render": function (data) {
                    return data ? data : '-';
                },
                "width": "10%",
                "className": "text-center"
            },
            {
                data: null,
                "render": function (data, type, row) {
                    let buttons = '';

                    if (row.status === 'Pending' && row.isLender) {
                        buttons += `
                            <button class="btn btn-success btn-sm me-1" 
                                aria-label="Approve request ${row.id}" 
                                onclick="handleAction('approve', ${row.id})">Approve</button>
                            <button class="btn btn-danger btn-sm me-1" 
                                aria-label="Reject request ${row.id}" 
                                onclick="handleAction('reject', ${row.id})">Reject</button>`;
                    }

                    if (row.status === 'Approved' && row.isBorrower) {
                        buttons += `<button class="btn btn-info btn-sm" 
                                  aria-label="Return request ${row.id}" 
                                  onclick="handleAction('return', ${row.id})">Return</button>`;
                    }

                    return buttons || '-';
                },
                "width": "20%",
                "className": "text-center"
            }
        ],
        "initComplete": function () {
            if (dataTable.rows().count() === 0) {
                $('#noDataMessage').show();
            }
        }
    });
}

function handleAction(action, requestId) {
    const url = `/Member/Borrow/${action === 'approve' ? 'ApproveRequest' :
        action === 'reject' ? 'RejectRequest' : 'ReturnBook'}`;

    Swal.fire({
        title: `Confirm ${action}`,
        text: `Are you sure you want to ${action} this request?`,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: `Yes, ${action} it!`
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "POST",
                data: { requestId: requestId },
                headers: {
                    "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        dataTable.ajax.reload(null, false);
                        toastr.success(response.message || `Request ${action}d successfully!`);
                    } else {
                        toastr.error(response.error || `Error ${action}ing request`);
                    }
                },
                error: function (xhr) {
                    const errorMessage = xhr.responseJSON?.error || xhr.statusText || `Error ${action}ing request`;
                    toastr.error(errorMessage);
                }
            });
        }
    });
}