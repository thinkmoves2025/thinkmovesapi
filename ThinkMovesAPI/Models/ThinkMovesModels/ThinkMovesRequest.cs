using Microsoft.AspNetCore.Mvc;

namespace PortfolioAPI.Models.ThinkMovesAIModels
{
    public class ThinkMovesRequest
    {
        [FromForm]
        public List<IFormFile> ScanImages { get; set;}
    }
}
