using Amazon.DynamoDBv2.DataModel;
namespace ThinkMovesAPI.Models.Players
{
    [DynamoDBTable("PlayersTable")]
    public class PlayerTableModel
    {
        [DynamoDBHashKey]
        public string PlayerID { get; set; }

        [DynamoDBProperty]
        public string Email { get; set; }

        [DynamoDBProperty]
        public string EmailVerified { get; set; }

        [DynamoDBProperty]
        public string CreatedAt { get; set; }
    }
}