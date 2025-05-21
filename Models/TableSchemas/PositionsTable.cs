using Amazon.DynamoDBv2.DataModel;
namespace ThinkMovesAPI.Models.TableSchemas
{
    [DynamoDBTable("PositionsTable")]
    public class PositionsTable
    {
        [DynamoDBHashKey]
        public string UserID { get; set; }

        [DynamoDBRangeKey]
        public int PositionID { get; set; }

        [DynamoDBProperty]
        public string PositionFEN { get; set; }

        [DynamoDBProperty]
        public string PositionNotes { get; set; }

        [DynamoDBProperty]
        public int PositionLikes { get; set; }

    }
}
