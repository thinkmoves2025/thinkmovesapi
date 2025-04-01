using ThinkMovesAPI.Models.SaveChessGameModels;

namespace ThinkMovesAPI.Services.Interface
{
    public interface ISaveChessGame
    {
        Task<SaveChessGameResponse> SaveGameAsync(SaveChessGameRequest saveChessGameRequest);
    }
}
