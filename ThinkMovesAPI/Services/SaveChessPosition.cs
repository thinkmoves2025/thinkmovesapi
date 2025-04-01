using Amazon.DynamoDBv2.DataModel;
using ThinkMovesAPI.Models.TableSchemas;
using ThinkMovesAPI.Models.SaveChessPositionModels;
using ThinkMovesAPI.Services.Interface;

namespace ThinkMovesAPI.Services
{
    public class SaveChessPosition : ISaveChessPosition
    {
        private readonly IDynamoDBContext _dynamoDBContext;

        public SaveChessPosition(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }
        public async Task<SaveChessPositionResponse> SavePositionAsync(SaveChessPositionRequest saveChessPositionRequest)
        {
            SaveChessPositionResponse saveChessPositionResponse = new SaveChessPositionResponse();

            PositionsTable positionsTable = new PositionsTable();

            positionsTable = saveChessPositionRequest.positionsTable;

            try
            {
                await _dynamoDBContext.SaveAsync(positionsTable);
            }
            catch (Exception ex) { 
                saveChessPositionResponse.saveChessPosRespVar = "Error saving position: " + ex.Message;
                return saveChessPositionResponse;
            
            }

            saveChessPositionResponse.saveChessPosRespVar = "Position saved successfully";
            return saveChessPositionResponse;
        }
    }
}
