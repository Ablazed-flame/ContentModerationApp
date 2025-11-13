using System.ComponentModel.DataAnnotations;

namespace ContentModerationApp.ViewModels
{
    public class ContentSubmissionViewModel
    {
        [Display(Name = "Text Content")]
        public string? TextContent { get; set; }

        [Display(Name = "Image File")]
        public IFormFile? ImageFile { get; set; }
    }
}
