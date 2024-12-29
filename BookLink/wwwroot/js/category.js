document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('createCategoryModal');
    const modalContent = document.getElementById('modalContent');

    modal.addEventListener('show.bs.modal', function () {
        fetch('/Category/Create')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Failed to load content');
                }
                return response.text();
            })
            .then(html => {
                modalContent.innerHTML = html;
            })
            .catch(error => {
                modalContent.innerHTML = '<p>Error loading content. Please try again later.</p>';
                console.error(error);
            });
    });

    modal.addEventListener('hidden.bs.modal', function () {
        modalContent.innerHTML = ''; // Clear content when modal is hidden
    });
});
