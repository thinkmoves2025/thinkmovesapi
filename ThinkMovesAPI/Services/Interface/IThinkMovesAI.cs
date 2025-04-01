using PortfolioAPI.Models.ThinkMovesAIModels;
using System.Threading.Tasks;
using ThinkMovesAPI.Models;

namespace PortfolioAPI.Services.Interfaces
{
    public interface IThinkMovesAI
    {
        Task<ThinkMovesResponse> ThinkMovesAIAsync(ThinkMovesRequest thinkMovesAIRequest);
    }
}
