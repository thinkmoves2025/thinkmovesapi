using Amazon.Textract.Model;

namespace ThinkMovesAPI.Models.HelperFuncModels
{
    public class ScanImagesHFResponse
    {
        public AnalyzeDocumentResponse frontPageResponse { get; set; }

        public AnalyzeDocumentResponse backPageResponse { get; set; }

        public List<string> Errors { get; set; } = new List<string>();
    }
}
