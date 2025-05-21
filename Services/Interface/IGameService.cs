using ThinkMovesAPI.Models.SaveChessGameModels;

namespace ThinkMovesAPI.Services.Interface
{
    public interface IGameService
    {
        Task<SaveChessGameResponse> SaveGameAsync(SaveChessGameRequest saveChessGameRequest);
    }
}
