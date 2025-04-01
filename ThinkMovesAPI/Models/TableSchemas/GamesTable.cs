using Amazon.DynamoDBv2.DataModel;
using ThinkMovesAPI.Models;
namespace ThinkMovesAPI.Models.TableSchemas
{
    [DynamoDBTable("GamesTable")]
    public class GamesTable
    {
        [DynamoDBHashKey]
        public string UserID { get; set; }

        [DynamoDBRangeKey]
        public int GameID { get; set; }

        [DynamoDBProperty]
        public string gameNotes { get; set; }

        [DynamoDBProperty]
        public int gameLikes { get; set; }

        [DynamoDBProperty]
        public string allMoves { get; set; }

        [DynamoDBProperty]
        public ThinkMovesChessGame gameSheetData { get; set; } = new ThinkMovesChessGame();

        [DynamoDBProperty]
        public List<string> gameImagesS3Location { get; set; }

    }
}
