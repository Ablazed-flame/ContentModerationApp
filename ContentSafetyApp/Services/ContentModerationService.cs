using ContentModerationApp.Models;

namespace ContentModerationApp.Services
{
    public class ContentModerationService: IContentModerationService
    {

        private readonly AzureContentSafetyService _azureContentSafetyService;
        private readonly CohereApiService _cohereApiService;
        private readonly IWebHostEnvironment _envvironment;

        public ContentModerationService(AzureContentSafetyService azureContentSafetyService
            ,CohereApiService cohereApiService
            ,IWebHostEnvironment envvironment)
        {
            _azureContentSafetyService = azureContentSafetyService;
            _cohereApiService = cohereApiService;
            _envvironment = envvironment;
        }

        public async Task<ModerationResult> AnalyzeTextAsync(TextContentItem textItem)
        {
            var textAnalysisResult = await _azureContentSafetyService.AnalyzeText(textItem.Text);

            string summary = "";

            if(textAnalysisResult.IsFlagged)
            {
                summary = await _cohereApiService.AnalyzeTextAsync(textItem.Text);
            }

            return new ModerationResult
            {
                IsFlagged = textAnalysisResult.IsFlagged,
                FlagReasons = textAnalysisResult.Reasons,
                ModerationSummary = textAnalysisResult.IsFlagged?summary:"Clean"
            };

        }
        public async Task<ModerationResult> AnalyzeImageAsync(ImageContentItem imageItem)
        {
            string imageFullPath = Path.Combine(_envvironment.WebRootPath, imageItem.ImagePath.TrimStart('/'));
            var imageAnalysisResult = await _azureContentSafetyService.AnalyzeImage(imageFullPath);
            string summary = "";
            if(imageAnalysisResult.IsFlagged)
            {
                summary = await _cohereApiService.AnalyzeImageAsync(imageFullPath);
            }
            return new ModerationResult
            {
                IsFlagged = imageAnalysisResult.IsFlagged,
                FlagReasons = imageAnalysisResult.Reasons,
                ModerationSummary = imageAnalysisResult.IsFlagged ? summary : "Clean"
            };
        }
    }
}
