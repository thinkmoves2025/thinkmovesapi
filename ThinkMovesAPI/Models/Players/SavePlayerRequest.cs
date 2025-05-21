namespace ThinkMovesAPI.Models.Players
{
    public class SavePlayerRequest
    {
        public string PlayerID { get; set; }
        public string Email { get; set; }
        public string EmailVerified { get; set; }
        public string CreatedAt { get; set; }
    }
}
