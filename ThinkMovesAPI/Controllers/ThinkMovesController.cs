using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Models.ThinkMovesAIModels;
using PortfolioAPI.Services.Interfaces;
using ThinkMovesAPI.Models.SaveChessGameModels;
using ThinkMovesAPI.Services.Interface;
using ThinkMovesAPI.Models.SaveChessPositionModels;

namespace ThinkMovesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ThinkMovesController : Controller
    {
        private IThinkMovesAI _thinkMovesAIService;
        private ISaveChessGame _saveChessGameService;
        private ISaveChessPosition _saveChessPositionService;

        public ThinkMovesController(IThinkMovesAI thinkMovesAIService, ISaveChessGame saveChessGameService, ISaveChessPosition saveChessPosition)
        {           
            this._thinkMovesAIService = thinkMovesAIService;
            this._saveChessGameService = saveChessGameService;
            this._saveChessPositionService = saveChessPosition;
        }

        //For now I can think of 4 endpoints
        //1. ThinkMovesAI -> which will run after clicking submit button

        //Everything below is related to the user.
        //2. Save Game button
        //3. Save Position button
        //4. Saving User Details

        [HttpPost("ThinkMovesAI")]
        [Consumes("multipart/form-data")]
        public async Task<ThinkMovesResponse> ThinkMovesAI([FromForm] ThinkMovesRequest thinkMovesAIRequest) => await _thinkMovesAIService.ThinkMovesAIAsync(thinkMovesAIRequest);


        [HttpPost("SaveChessGame")]
        public async Task<SaveChessGameResponse> SaveChessGame(SaveChessGameRequest saveChessGameRequest) => await _saveChessGameService.SaveGameAsync(saveChessGameRequest);


        [HttpPost("SaveChessPosition")]
        public async Task<SaveChessPositionResponse> SaveChessPosition(SaveChessPositionRequest saveChessPositionRequest) => await _saveChessPositionService.SavePositionAsync(saveChessPositionRequest);

    }
}
