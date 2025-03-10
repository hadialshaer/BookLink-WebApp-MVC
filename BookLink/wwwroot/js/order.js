﻿var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("inprocess")) {
        loadDataTable("inprocess");
    } else {
        if (url.includes("completed")) {
            loadDataTable("completed");
        } else {
            if (url.includes("pending")) {
                loadDataTable("pending");
            }
            else {
                if (url.includes("approved")) {
                    loadDataTable("approved");
                }
                else
                    loadDataTable("all");

            }
        }
    }

});

function loadDataTable(status) {
    dataTable = $('#tableData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            {
                data: 'id',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'name',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'phoneNumber',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'user.email',
                "width": "20%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'orderStatus',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },
            {
                data: 'orderTotal',
                "width": "15%",
                "className": "text-center",
                "render": function (data, type, row) {
                    return data || "N/A";
                }
            },

            {
                data: 'id',
                "render": function (data) {
                    return `
                        <div class="d-flex justify-content-center">
                            <a href="/admin/order/details?orderId=${data}" class="btn btn-primary btn-sm">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                        </div>`;
                },
                "width": "20%",
                "className": "text-center"
            }
        ]
    });
}