$(document).ready(function () {
    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();

    // Search functionality
    $('#searchInput').keyup(function () {
        var value = $(this).val().toLowerCase();
        $('#submissionsTable tbody tr').filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1)
        });
    });

    // Status filter buttons would be implemented here
});