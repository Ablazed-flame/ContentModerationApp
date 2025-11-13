$(document).ready(function () {
    // Cache selectors for performance
    const $form = $('#submissionForm');
    const $submitBtn = $('#submitBtn');
    const $imageInput = $('#imageUpload');
    const $textContent = $('#TextContent');
    const $previewSection = $('#imagePreview');
    const $previewImg = $('#previewImage');
    const MAX_LENGTH = 5000;

    // 1. Image Preview Functionality
    $imageInput.change(function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();
            reader.onload = function (e) {
                $previewImg.attr('src', e.target.result);
                $previewSection.fadeIn(); // Smooth fade in
            }
            reader.readAsDataURL(this.files[0]);
        } else {
            $previewSection.hide();
        }
    });

    // 2. Remove Image Button
    $('#removeImage').click(function () {
        $imageInput.val(''); // Clear the input
        $previewSection.fadeOut(); // Smooth fade out
    });

    // 3. Character Counter & Limit
    // Dynamically add the counter text if it doesn't exist
    if ($('#charCounter').length === 0) {
        $textContent.after(`<div id="charCounter" class="text-end text-muted small mt-1">0 / ${MAX_LENGTH}</div>`);
    }

    $textContent.on('input', function () {
        let currentText = $(this).val();
        let currentLength = currentText.length;

        // Hard limit
        if (currentLength > MAX_LENGTH) {
            $(this).val(currentText.substring(0, MAX_LENGTH));
            currentLength = MAX_LENGTH;
        }

        // Update visual counter
        $('#charCounter').text(`${currentLength} / ${MAX_LENGTH}`);
    });

    // 4. Handle "Reset" Button
    // If user clicks Reset, we need to manually clear the preview and counter
    $form.on('reset', function () {
        setTimeout(function () {
            $previewSection.hide();
            $('#charCounter').text(`0 / ${MAX_LENGTH}`);
            // Reset button state just in case
            $submitBtn.prop('disabled', false).html('<i class="fas fa-paper-plane me-2"></i>Submit for Analysis');
            // Remove error messages
            $('#custom-error-msg').remove();
        }, 10);
    });

    // 5. Form Submission
    $form.submit(function (e) {
        // Remove any existing custom error alerts
        $('#custom-error-msg').remove();

        // --- CUSTOM VALIDATION: One or the other required ---
        const textVal = $textContent.val().trim();
        const hasImage = $imageInput[0].files && $imageInput[0].files.length > 0;

        if (!textVal && !hasImage) {
            e.preventDefault(); // Stop submission

            // Insert a nice Bootstrap error alert at the top of the form
            $form.prepend(`
                <div id="custom-error-msg" class="alert alert-danger alert-dismissible fade show" role="alert">
                    <i class="fas fa-exclamation-circle me-2"></i> 
                    Please enter <strong>Text</strong> OR upload an <strong>Image</strong> to submit.
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </div>
            `);

            // Scroll to the top so the user sees the error
            $('html, body').animate({ scrollTop: $form.offset().top - 80 }, 300);
            return false;
        }

        // --- STANDARD VALIDATION: Check Data Annotations (Max length, file type) ---
        if (!$(this).valid()) {
            // If ASP.NET validation fails, do nothing (errors will show)
            return false;
        }

        // --- SUCCESS STATE ---
        // Disable button and show spinner
        $submitBtn.prop('disabled', true);
        $submitBtn.html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span> Processing...');
    });
});