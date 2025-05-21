using ThinkMovesAPI.Models.Players;

namespace ThinkMovesAPI.Services.Interface
{
    public interface IPlayerService
    {
        Task<SavePlayerResponse> SavePlayerAsync(SavePlayerRequest savePlayerRequest);
    }
}
