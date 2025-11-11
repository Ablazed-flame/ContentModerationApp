using Azure.AI.ContentSafety;
using ContentModerationApp.DTOs;

namespace ContentModerationApp.Services
{
    public class AzureContentSafetyService
    {

        private readonly ContentSafetyClient _safetyClient;


        public AzureContentSafetyService(ContentSafetyClient safetyClient)
        {
            _safetyClient = safetyClient;
        }

        public async Task<TextAnalysisResult> AnalyzeText(string inputText)
        {
            var result = new TextAnalysisResult();
            if (string.IsNullOrWhiteSpace(inputText))
                return result;
            var textOptions = new AnalyzeTextOptions(inputText);
            var textResponse = await _safetyClient.AnalyzeTextAsync(textOptions);


            foreach (var category in textResponse.Value.CategoriesAnalysis)
            {
                if (category.Severity > 0)
                {
                    result.Reasons += $"Text flagged for {category.Category} (Severity: {category.Severity}). ";
                    result.IsFlagged = true;
                }
            }
            return result;
        }

        public async Task<ImageAnalysisResult> AnalyzeImage(string imageFullPath)
        {
            var result = new ImageAnalysisResult();
            if (string.IsNullOrEmpty(imageFullPath))
                return result;

            if (!System.IO.File.Exists(imageFullPath))
                return result;

            var imageBytes = await System.IO.File.ReadAllBytesAsync(imageFullPath);
            var imageData = new ContentSafetyImageData(BinaryData.FromBytes(imageBytes));
            var imageOptions = new AnalyzeImageOptions(imageData);
            var imageResponse = await _safetyClient.AnalyzeImageAsync(imageOptions);

            foreach (var category in imageResponse.Value.CategoriesAnalysis)
            {
                if (category.Severity > 0)
                {
                    result.Reasons += $"Image flagged for {category.Category} (Severity: {category.Severity}). ";
                    result.IsFlagged = true;
                }
            }
            return result;

        }


    }
}
