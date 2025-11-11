namespace ContentModerationApp.ViewModels
{
    public class AdminOverrideInputViewModel
    {
        public int Id { get; set; }
        public bool? AdminOverrideFlag { get; set; } // The new decision
        public string AdminOverrideNote { get; set; }
    }
}
