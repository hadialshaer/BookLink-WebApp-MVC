function DeleteCartItem(url) {
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
                    location.reload();
                    toastr.success(data.message || "Cart item removed successfully.");
                },
                error: function () {
                    toastr.error("Failed to delete the cart item. Please try again.");
                }
            });
        }
    });
}
