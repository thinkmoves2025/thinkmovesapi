using Microsoft.AspNetCore.Mvc;
using ThinkMovesAPI.Models;
using ThinkMovesAPI.Models.SaveChessGameModels;
using ThinkMovesAPI.Services.Interface;
using ThinkMovesAPI.Models.SaveChessPositionModels;
using ThinkMovesAPI.Models.ThinkMovesAIModels;
using ThinkMovesAPI.Services.Interfaces;

namespace ThinkMovesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThinkMovesController : Controller
    {
        private IThinkMovesAI _thinkMovesAIService;
        private IGameService _gameService;
        private IPositionService _positionService;

        public ThinkMovesController(IThinkMovesAI thinkMovesAIService, IGameService gameService, IPositionService positionService)
        {           
            this._thinkMovesAIService = thinkMovesAIService;
            this._gameService = gameService;
            this._positionService =positionService;
        }

        //For now I can think of 4 endpoints
        //1. ThinkMovesAI -> which will run after clicking submit button

        //Everything below is related to the user.
        //2. Save Game button
        //3. Save Position button   
        //4. Saving User Details

        [HttpPost("ThinkMovesAI")]
        //[Consumes("multipart/form-data")]
        public async Task<ThinkMovesResponse> ThinkMovesAI([FromForm] List<IFormFile> gameImages) { 
        
        ThinkMovesResponse thinkMovesResponse = new ThinkMovesResponse();

            thinkMovesResponse = await _thinkMovesAIService.ThinkMovesAIAsync(gameImages);
            //thinkMovesResponse.response = "Images Received";
            return thinkMovesResponse;
        
        }


        [HttpPost("SaveChessGame")]
        public async Task<SaveChessGameResponse> SaveChessGame(SaveChessGameRequest saveChessGameRequest) => await _gameService.SaveGameAsync(saveChessGameRequest);


        [HttpPost("SaveChessPosition")]
        public async Task<SaveChessPositionResponse> SaveChessPosition(SaveChessPositionRequest saveChessPositionRequest) => await _positionService.SavePositionAsync(saveChessPositionRequest);

    }
}
