namespace ThinkMovesAPI.Models.HelperFuncModels
{
    public class LambdaAndCombineResponse
    {
        public string response { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
