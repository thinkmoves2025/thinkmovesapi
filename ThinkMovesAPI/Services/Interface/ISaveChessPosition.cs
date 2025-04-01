using Amazon.DynamoDBv2.DataModel;
using ThinkMovesAPI.Models.SaveChessPositionModels;

namespace ThinkMovesAPI.Services.Interface
{
    public interface ISaveChessPosition
    {
        Task<SaveChessPositionResponse> SavePositionAsync(SaveChessPositionRequest saveChessPositionRequest);
    }
}
