﻿using Amazon.DynamoDBv2.DataModel;
using ThinkMovesAPI.Models.SaveChessGameModels;
using ThinkMovesAPI.Models.TableSchemas;
using ThinkMovesAPI.Services.Interface;

namespace ThinkMovesAPI.Services

{
    public class GameService : IGameService
    {
        private readonly IDynamoDBContext _dynamoDBContext;

        public GameService(IDynamoDBContext dynamoDBContext)
        {
            _dynamoDBContext = dynamoDBContext;
        }
        public async Task<SaveChessGameResponse> SaveGameAsync(SaveChessGameRequest saveChessGameRequest)
        {
            SaveChessGameResponse saveChessGameResponse = new SaveChessGameResponse();

            GamesTable gamesTable = new GamesTable();

            gamesTable = saveChessGameRequest.gamesTable;


            try
            {
                await _dynamoDBContext.SaveAsync(gamesTable);
            }
            catch (Exception ex)
            {
                saveChessGameResponse.errorMessages.Add("Error saving game: " + ex.Message);
                return saveChessGameResponse;
            }

            saveChessGameResponse.saveChessGameResponse = "Game saved successfully";


            return saveChessGameResponse;
        }
    }
}
