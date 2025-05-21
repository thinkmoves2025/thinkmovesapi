using ThinkMovesAPI.Models;
using ThinkMovesAPI.Models.HelperFuncModels;
using ThinkMovesAPI.Models.ThinkMovesAIModels;
using ThinkMovesAPI.Services.Interfaces;

namespace ThinkMovesAPI.Services
{

    public class ThinkMovesAI : IThinkMovesAI
    {
        private readonly IHelperFuncService _helperFuncService;

        public ThinkMovesAI(IHelperFuncService helperFuncService)
        {
            _helperFuncService = helperFuncService;
        }

        //Work flow of the service

        //Scan the images and get the front end and backend response
        //Get all the data from frontend and backend and combine them
        //Then run the lambda function using the combined data
        //and return the response


        public async Task<ThinkMovesResponse> ThinkMovesAIAsync(List<IFormFile> gameImages)
        {
            ThinkMovesResponse thinkMovesAIResponse = new ThinkMovesResponse();

            //First we will scan the images and get the front end and backend response
            ScanImagesHFResponse scanImagesHFResponse = new ScanImagesHFResponse();

            //Check if images are null

            if (gameImages == null || gameImages.Count == 0)
            {
                thinkMovesAIResponse.Errors.Add("No images were uploaded.");
                return thinkMovesAIResponse;
            }


            //Here images are not null

            else
            {
                ScanImagesHFRequest scanImagesHFRequest = new ScanImagesHFRequest();
                scanImagesHFRequest.ScanImagesHFReqVar = gameImages;


                //This function will scan the images and read the text and pgn format of the game.

                scanImagesHFResponse = await _helperFuncService.ScanImagesHFAsync(scanImagesHFRequest);

                if (scanImagesHFResponse.Errors.Count == 0)
                {

                    ThinkMovesChessGame thinkMovesChessGame = new ThinkMovesChessGame();

                    //Getting all the chess moves and add to ChessGame object
                    //Combine the PGN formats and return an orderedList of moves and other game info.
                    var completeOrderedList = _helperFuncService.ScannedGameDataHF(scanImagesHFResponse.frontPageResponse, scanImagesHFResponse.backPageResponse, thinkMovesChessGame);

                    //Convert the ordered list to JSON format
                    string allMovesJSON = _helperFuncService.ConvertToJSONHF(completeOrderedList);
                    
                    //Now we will run the lambda function using the combined data and ChessGame object
                    //We will get JSON containing Correct MOves, RemainingMvoes, Error,suggested moves,etc.
                    //We have to return this JSON to the front end.
                    LambdaAndCombineResponse lambdaAndCombineResponse = await _helperFuncService.LambdaAndCombine(allMovesJSON, thinkMovesChessGame);

                    if (lambdaAndCombineResponse.Errors.Count == 0)
                    {
                       thinkMovesAIResponse.response =  lambdaAndCombineResponse.response;
                    }

                    else
                    {
                        thinkMovesAIResponse.Errors = lambdaAndCombineResponse.Errors;
                    }

                }
                return thinkMovesAIResponse;
            }
        }
    }
}
