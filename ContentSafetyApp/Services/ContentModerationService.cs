using ContentModerationApp.DTOs;
using ContentModerationApp.Models;
using System.Text;

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

            var reasons = new StringBuilder();
            bool flag = false;
  
            foreach (var severity in textAnalysisResult.SeverityScores)
            {
                if (severity.Value > 0)
                {
                    reasons.Append($"Text flagged for {severity.Key} (Severity: {severity.Value}).");
                    flag = true; 
                }
            }
            string summary = "";
            if (flag==true)
            {
                summary = await _cohereApiService.AnalyzeTextAsync(textItem.Text);
            }
            return new ModerationResult
            {
                IsFlagged = flag,
                FlagReasons = reasons.ToString(),
                ModerationSummary = flag?summary:"Clean"
            };

        }
        public async Task<ModerationResult> AnalyzeImageAsync(ImageContentItem imageItem)
        {
            string imageFullPath = Path.Combine(_envvironment.WebRootPath, imageItem.ImagePath.TrimStart('/'));
            var imageAnalysisResult = await _azureContentSafetyService.AnalyzeImage(imageFullPath);
            var reasons = new StringBuilder();
            bool flag = false;

            foreach (var severity in imageAnalysisResult.SeverityScores)
            {
                if (severity.Value > 0)
                {
                    reasons.Append($"Image flagged for {severity.Key} (Severity: {severity.Value}).");
                    flag = true;
                }
            }
            string summary = "";
            if(flag)
            {
                summary = await _cohereApiService.AnalyzeImageAsync(imageFullPath);
            }
            return new ModerationResult
            {
                IsFlagged = flag,
                FlagReasons = reasons.ToString().Trim(),
                ModerationSummary = flag ? summary : "Clean"
            };
        }
    }
}
