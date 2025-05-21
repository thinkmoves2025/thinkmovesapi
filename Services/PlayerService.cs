using Amazon.DynamoDBv2.DataModel;
using ThinkMovesAPI.Models.Players;
using ThinkMovesAPI.Models.SaveChessPositionModels;
using ThinkMovesAPI.Models.TableSchemas;
using ThinkMovesAPI.Services.Interface;

namespace ThinkMovesAPI.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IDynamoDBContext _dynamoDBContext;

        public PlayerService(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }
        public async Task<SavePlayerResponse> SavePlayerAsync(SavePlayerRequest savePlayerRequest)
        {
            SavePlayerResponse savePlayerResponse = new SavePlayerResponse();

            PlayerTableModel model = new PlayerTableModel();
            model.CreatedAt = savePlayerRequest.CreatedAt;
            model.EmailVerified = savePlayerRequest.EmailVerified;
            model.PlayerID = savePlayerRequest.PlayerID;
            model.Email = savePlayerRequest.Email;

            try
            {
                await _dynamoDBContext.SaveAsync(model);
            }
            catch (Exception ex)
            {
                savePlayerResponse.SavePlayerResponseVar = "Error saving position: " + ex.Message;
                return savePlayerResponse;
            }

            savePlayerResponse.SavePlayerResponseVar = "Player saved successfully";
            return savePlayerResponse;
        }
    }
}
