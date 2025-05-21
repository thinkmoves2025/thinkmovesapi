using Microsoft.AspNetCore.Mvc;
using ThinkMovesAPI.Models.Players;
using ThinkMovesAPI.Models.SaveChessGameModels;
using ThinkMovesAPI.Services;
using ThinkMovesAPI.Services.Interface;
using ThinkMovesAPI.Services.Interfaces;

namespace ThinkMovesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : Controller
    {
        private IPlayerService _playerService;
        public PlayerController (IPlayerService playerService)
        {
            this._playerService = playerService;
        }

        [HttpPost("SavePlayerDetails")]
        public async Task<SavePlayerResponse> SavePlayer(SavePlayerRequest savePlayerRequest) => await _playerService.SavePlayerAsync(savePlayerRequest);
    }
}
