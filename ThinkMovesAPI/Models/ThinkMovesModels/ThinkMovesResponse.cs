namespace PortfolioAPI.Models.ThinkMovesAIModels
{
    public class ThinkMovesResponse
    {
        public string response { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
