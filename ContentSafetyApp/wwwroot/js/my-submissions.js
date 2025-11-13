
    $(document).ready(function() {
        // Initialize tooltips
        $('[data-bs-toggle="tooltip"]').tooltip();

    // Format dates with humanize
    $('.humanize').each(function() {
                const date = new Date($(this).text());
    $(this).text(humanizeDuration(Date.now() - date, {
        round: true,
    largest: 1
                }) + ' ago');
            });

    // View details button handler
    $('.view-details').click(function() {
                const submissionId = $(this).data('id');
    // Implement your detail view logic here
    console.log('View details for submission:', submissionId);
            });
        });
