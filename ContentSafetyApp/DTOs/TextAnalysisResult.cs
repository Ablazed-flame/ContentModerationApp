using Azure.AI.ContentSafety;
using System.ComponentModel;

namespace ContentModerationApp.DTOs
{
    public class TextAnalysisResult
    {

        public Dictionary<TextCategory, int> SeverityScores { get; set; } = new();
        //public int? HateSeverity { get; set; }
        //public int? SexualSeverity { get; set; }
        //public int? SelfHarmSeverity { get; set; }
        //public int? ViolenceSeverity { get; set; }
    }
}
