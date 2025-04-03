using ThinkMovesAPI.Models.HelperFuncModels;
using ThinkMovesAPI.Models;
using Amazon.Runtime;
using Amazon.Textract;
using Amazon.Textract.Model;
using Newtonsoft.Json;
using ThinkMovesAPI.Models.HelperFuncModels;
using ThinkMovesAPI.Services.Interfaces;
using System.Text;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System;


namespace ThinkMovesAPI.Services
{
    public class HelperFuncService : IHelperFuncService
    {

        //We are getting images from user named as scanImages -> THis is the input
        //We are reading the images and saving into s3 bucket
        //We are getting frontPage and backPage images

        //We will be running a function "ThinkMoveCompleteScan" which will take frontPage and backPage Responses as input
        //and will return the orderedList of moves and other game info

        //Then we convert the orderedList into a json format

        //Then AWS Lambda function will be called with the json input and will return the jsonResponse --> Look in detail.

        public async Task<ScanImagesHFResponse> ScanImagesHFAsync(ScanImagesHFRequest scanImagesHFRequest)
        {
            ScanImagesHFResponse scanImagesHFResponse = new ScanImagesHFResponse();

            // Get the AWS configuration            

            var awsAccessKeyID = System.Environment.GetEnvironmentVariable("AWS_AccessKeyId");
            var awsSecretAccessKey = System.Environment.GetEnvironmentVariable("AWS_SecretAccessKey");
            var awsCreds = new BasicAWSCredentials(awsAccessKeyID, awsSecretAccessKey);
            var textrclient = new AmazonTextractClient(awsCreds, Amazon.RegionEndpoint.USEast1);

            AnalyzeDocumentResponse frontPageResponse = null;
            AnalyzeDocumentResponse backPageResponse = null;
        

            foreach (var scanImage in scanImagesHFRequest.ScanImagesHFReqVar)
            {
                var memoryStream = new MemoryStream();
                await scanImage.CopyToAsync(memoryStream);
                var scannedImagesRequest = new AnalyzeDocumentRequest
                {
                    Document = new Document
                    {
                        Bytes = memoryStream // Ensure the memory stream is converted to a byte array
                    },
                    FeatureTypes = new List<string> { "LAYOUT" }
                };

                AnalyzeDocumentResponse scannedImageResponse = await textrclient.AnalyzeDocumentAsync(scannedImagesRequest);

                //Checking the first block of the response
                var firstBlock = scannedImageResponse.Blocks.FirstOrDefault(b => b.BlockType == "LINE");

                //Check if the first block contains "ChessGrow" text to determine the page layout
                if (firstBlock != null && firstBlock.Text.Trim() == "ChessGrow")
                {
                    if (frontPageResponse == null)
                    {
                        //Console.WriteLine("Front Page");
                        frontPageResponse = scannedImageResponse;
                        scanImagesHFResponse.frontPageResponse = frontPageResponse;
                    }
                    else
                    {
                        //Console.WriteLine("Invalid Images - 2 frontpages");
                        break;
                    }
                }
                else if (scanImagesHFRequest.ScanImagesHFReqVar.Count == 2 && backPageResponse == null)
                {
                    backPageResponse = scannedImageResponse;
                    scanImagesHFResponse.backPageResponse = backPageResponse;
                }
                else
                {
                    scanImagesHFResponse.Errors.Add("Invalid Images, Enter Again");
                }

            }


            //We have FrontPage and BackPage responses
            //We will be running a function "ThinkMoveCompleteScan" which will take frontPage and backPage Responses as input

            //Use CHessGameObject here and not in ThinkMoveCompleteScan(). 
            //We need that GameObject, which contains metadata of the game, in the final response
            //Just add all the attributes named as "GameMetadata" in the GameObject and return it in the final response
            
            return scanImagesHFResponse;
        }

        public IOrderedEnumerable<KeyValuePair<int, (string, string)>> ScannedGameDataHF(AnalyzeDocumentResponse frontPageResponse, AnalyzeDocumentResponse backPageResponse, ThinkMovesChessGame thinkMovesChessGame)
        {
            IOrderedEnumerable<KeyValuePair<int, (string, string)>> totalSortedPGN = null;

            if (frontPageResponse != null)
            {
                var frontPageSortedPGN = ThinkMoveFrontPage(frontPageResponse, thinkMovesChessGame);
                if (backPageResponse != null)
                {
                    var backPageSortedPGN = ThinkMoveBackPage(backPageResponse, thinkMovesChessGame);
                    totalSortedPGN = frontPageSortedPGN.Concat(backPageSortedPGN).OrderBy(kvp => kvp.Key);
                }
                else
                {
                    totalSortedPGN = frontPageSortedPGN;
                }                
            }
            return totalSortedPGN;
        }

        public string ConvertToJSONHF(IOrderedEnumerable<KeyValuePair<int, (string, string)>> orderedEnumerable)
        {
            // Create the inner dictionary for ThinkMoveScannedGame
            var dictionary = orderedEnumerable.ToDictionary(
                kvp => kvp.Key.ToString(), // Keys must be strings for JSON
                kvp => new { whiteMove = kvp.Value.Item1, blackMove = kvp.Value.Item2 }
            );

            // Serialize the ThinkMovesChessGame object as a JSON string
            //string gameMetadataJson = JsonConvert.SerializeObject(new { GameMetadata = thinkMovesChessGame });

            // Serialize the ThinkMoveScannedGame object as a JSON string
            string movesJson = JsonConvert.SerializeObject(new { ThinkMoveScannedGame = dictionary });

            // Wrap both JSON strings under the "body" key
            var outerObject = new
            {
                body = new
                {
                    //GameMetadata = thinkMovesChessGame, // Include GameMetadata object
                    ThinkMoveScannedGame = dictionary  // Include the moves dictionary
                }
            };

            // Serialize the outer object
            string jsonString = JsonConvert.SerializeObject(outerObject, Formatting.Indented);

            return jsonString;
        }     

        private static IOrderedEnumerable<KeyValuePair<int, (string, string)>> ThinkMoveBackPage(AnalyzeDocumentResponse backPageResponse, ThinkMovesChessGame chessGame)
        {
            Dictionary<int, (string WhiteMove, string BlackMove)> moves = new Dictionary<int, (string, string)>();
            StringBuilder otherTextBuilder = new StringBuilder();  // Start with existing other text

            // Skip the first 7 lines
            var relevantBlocks = backPageResponse.Blocks.Skip(7).ToList();

            for (int i = 0; i < relevantBlocks.Count; i++)
            {
                if (relevantBlocks[i].BlockType == "LINE")
                {
                    string text = relevantBlocks[i].Text.Trim();
                    if (int.TryParse(text, out int currentMoveNumber) && currentMoveNumber >= 39 && currentMoveNumber <= 80)
                    {
                        string whiteMove = string.Empty;
                        string blackMove = string.Empty;

                        if (i + 1 < relevantBlocks.Count && relevantBlocks[i + 1].BlockType == "LINE" && !int.TryParse(relevantBlocks[i + 1].Text.Trim(), out _))
                        {
                            whiteMove = relevantBlocks[i + 1].Text.Trim();
                            i++;
                        }

                        if (i + 1 < relevantBlocks.Count && relevantBlocks[i + 1].BlockType == "LINE" && !int.TryParse(relevantBlocks[i + 1].Text.Trim(), out _))
                        {
                            blackMove = relevantBlocks[i + 1].Text.Trim();
                            i++;
                        }

                        moves[currentMoveNumber] = (whiteMove, blackMove);
                    }
                }
            }
            //Sorting moves in order
            var sortedPgnData = moves.OrderBy(kvp => kvp.Key);

            return sortedPgnData;
        }

        private static IOrderedEnumerable<KeyValuePair<int, (string, string)>> ThinkMoveFrontPage(AnalyzeDocumentResponse frontPageResponse, ThinkMovesChessGame chessGame)
        {

            //Make use of chessGame object to save the metadata of the game


            StringBuilder otherTextBuilder = new StringBuilder();
            string currentKey = null;
            bool startCollectingOtherText = false;
            bool beforeHash = true;
            int blackOccurrences = 0;

            //We will save this data in database (Like initial database where we dont know if its scanned correctly or not)
            //When the scan we will be complete we will save the data in the final database
            Dictionary<int, (string, string)> moves = new Dictionary<int, (string, string)>();

            foreach (var block in frontPageResponse.Blocks)
            {
                if (block.BlockType == "LINE")
                {
                    string text = block.Text.Trim();
                    //Console.WriteLine(text); // For debugging

                    if (beforeHash)
                    {
                        if (text == "#")
                        {
                            beforeHash = false;
                            continue;
                        }

                        if (text == "ChessGrow")
                        {
                            chessGame.PageType = "ChessGrow";
                        }
                        else if (text == "Date")
                        {
                            currentKey = "Date";
                        }
                        else if (text == "Board")
                        {
                            currentKey = "Board";
                        }
                        else if (text == "Event")
                        {
                            currentKey = "Event";
                        }
                        else if (text == "Round")
                        {
                            currentKey = "Round";
                        }
                        else if (text == "Black")
                        {
                            currentKey = "BlackPlayer";
                        }
                        else if (text == "Rating" && currentKey == "BlackPlayer")
                        {
                            currentKey = "BlackRating";
                        }
                        else if (text == "White")
                        {
                            currentKey = "WhitePlayer";
                        }
                        else if (text == "Rating" && currentKey == "WhitePlayer")
                        {
                            currentKey = "WhiteRating";
                        }
                        else if (currentKey != null)
                        {
                            AssignCurrentValue(chessGame, currentKey, text);
                            // Only reset currentKey if it's not a player key
                            if (currentKey != "BlackPlayer" && currentKey != "WhitePlayer")
                            {
                                currentKey = null;
                            }
                        }
                    }
                    else
                    {
                        if (text == "Black")
                        {
                            blackOccurrences++;
                            if (blackOccurrences == 2)
                            {
                                startCollectingOtherText = true;
                                continue;
                            }
                        }

                        if (startCollectingOtherText)
                        {
                            if (int.TryParse(text, out int moveNumber))
                            {
                                moves[moveNumber] = ("", ""); // Initialize with empty strings
                            }
                            else if (moves.Count > 0)
                            {
                                var lastMove = moves.Last();
                                if (string.IsNullOrEmpty(lastMove.Value.Item1))
                                {
                                    moves[lastMove.Key] = (text, lastMove.Value.Item2);
                                }
                                else if (string.IsNullOrEmpty(lastMove.Value.Item2))
                                {
                                    moves[lastMove.Key] = (lastMove.Value.Item1, text);
                                }
                            }
                        }
                    }
                }
            }

          
            for (int i = 1; i <= 38; i++)
            {
                if (!moves.ContainsKey(i))
                {
                    moves[i] = ("", "");
                }
                otherTextBuilder.AppendLine($"{i}");
                otherTextBuilder.AppendLine(moves[i].Item1);
                otherTextBuilder.AppendLine(moves[i].Item2);
            }

            var sortedPgnData = moves.OrderBy(kvp => kvp.Key);

            return sortedPgnData;
        }

        private static void AssignCurrentValue(ThinkMovesChessGame chessGame, string currentKey, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (currentKey)
                {
                    case "Date":
                        chessGame.Date = value;
                        break;
                    case "Board":
                        chessGame.Board = value;
                        break;
                    case "Event":
                        chessGame.Event = value;
                        break;
                    case "Round":
                        chessGame.Round = value;
                        break;
                    case "BlackPlayer":
                        chessGame.BlackPlayer = value;
                        break;
                    case "WhitePlayer":
                        chessGame.WhitePlayer = value;
                        break;
                    case "BlackRating":
                        chessGame.BlackRating = value;
                        break;
                    case "WhiteRating":
                        chessGame.WhiteRating = value;
                        break;
                }
            }
        }

        public async Task<LambdaAndCombineResponse> LambdaAndCombine(string jsonResult, ThinkMovesChessGame thinkMovesChessGame)
        {
            LambdaAndCombineResponse lambdaAndCombineResponse = new LambdaAndCombineResponse();

            string combinedJsonResponse = "";

            string functionName = System.Environment.GetEnvironmentVariable("ThinkMovesLambdaFunc");
            string jsonResponse = "";

            var awsAccessKeyID = System.Environment.GetEnvironmentVariable("AWS_AccessKeyId");
            var awsSecretAccessKey = System.Environment.GetEnvironmentVariable("AWS_SecretAccessKey");
            var awsCreds = new BasicAWSCredentials(awsAccessKeyID, awsSecretAccessKey);

            AmazonLambdaClient amazonLambdaClient = new AmazonLambdaClient(awsCreds, RegionEndpoint.USEast1); //lambda is in US-east-1 Lambda

            var request = new InvokeRequest
            {
                FunctionName = functionName,
                Payload = jsonResult
            };

            InvokeResponse response = await amazonLambdaClient.InvokeAsync(request);
            


            // Check if the response was successful
            //Lambda return a JSON response will have complete PGN including metadata of the game and other info like correct moves, errors, suggested moves, remainingPGN, LastValidFEN
            if (response.Payload != null)
            {
                using (var reader = new StreamReader(response.Payload))
                {
                    // Read the JSON string from the payload
                    jsonResponse = await reader.ReadToEndAsync();
                }
                // Deserialize into a C# object or process as needed
                //var lambdaResponse = JsonConvert.DeserializeObject(jsonResponse);

                var lambdaResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);


                // Create the game metadata JSON object
                var gameMetadataJson = new
                {
                    GameMetaDataJSON = JsonConvert.SerializeObject(new
                    {
                        GameMetadata = thinkMovesChessGame
                    })
                };


                //Here we have to return
                //GameMetaDataJSON
                //

                // Deserialize the body from the Lambda response
                if (lambdaResponse.ContainsKey("body"))
                {
                    var body = JsonConvert.DeserializeObject<Dictionary<string, object>>(lambdaResponse["body"].ToString());

                    // Add GameMetaDataJSON to the body
                    body["GameMetaDataJSON"] = gameMetadataJson.GameMetaDataJSON;

                    // Update the body in the Lambda response
                    lambdaResponse["body"] = JsonConvert.SerializeObject(body);
                }
                else {                     // Handle the case where the body key is not present
                    //Console.WriteLine("Lambda response does not contain 'body' key.");
                    lambdaAndCombineResponse.Errors.Add("Lambda response does not contain 'body' key.");
                    
                }

                // Serialize the updated Lambda response back to JSON
                combinedJsonResponse = JsonConvert.SerializeObject(lambdaResponse, Formatting.Indented);

                lambdaAndCombineResponse.response = combinedJsonResponse;


            }
            else
            {
                // Handle the case where the response payload is null
                //Console.WriteLine("Lambda function returned null payload.");
                lambdaAndCombineResponse.Errors.Add("Lambda function returned null payload.");
                
            }

            return lambdaAndCombineResponse;
        }
    }
}
