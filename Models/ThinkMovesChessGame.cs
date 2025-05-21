namespace ThinkMovesAPI.Models
{
    public class ThinkMovesChessGame
    {
        public string PageType { get; set; }
        public string Event { get; set; }
        public string Round { get; set; }
        public string Board { get; set; }
        public string Date { get; set; }
        public string BlackPlayer { get; set; }
        public string WhitePlayer { get; set; }
        public string BlackRating { get; set; }
        public string WhiteRating { get; set; }
        public string OtherText { get; set; } 
    }
}
