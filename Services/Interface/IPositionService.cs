using ThinkMovesAPI.Models.SaveChessPositionModels;

namespace ThinkMovesAPI.Services.Interface
{
    public interface IPositionService
    {
        Task<SaveChessPositionResponse> SavePositionAsync(SaveChessPositionRequest saveChessPositionRequest);
    }
}
