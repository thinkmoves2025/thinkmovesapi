using ThinkMovesAPI.Models.ThinkMovesAIModels;
using System.Threading.Tasks;
using ThinkMovesAPI.Models;

namespace ThinkMovesAPI.Services.Interfaces
{
    public interface IThinkMovesAI
    {
        Task<ThinkMovesResponse> ThinkMovesAIAsync(List<IFormFile> gameImages);
    }
}
