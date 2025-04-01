using ThinkMovesAPI.Models;
using PortfolioAPI.Models.HelperFuncModels;
namespace PortfolioAPI.Services.Interfaces;

using Amazon.Textract.Model;
using System.Threading.Tasks;
using ThinkMovesAPI.Models;
using ThinkMovesAPI.Models.HelperFuncModels;

public interface IHelperFuncService
    {
        IOrderedEnumerable<KeyValuePair<int, (string, string)>> ScannedGameDataHF(AnalyzeDocumentResponse frontPageResponse, AnalyzeDocumentResponse backPageResponse, ThinkMovesChessGame thinkMovesChessGame);
        string ConvertToJSONHF(IOrderedEnumerable<KeyValuePair<int, (string, string)>> orderedEnumerable);
        Task<ScanImagesHFResponse> ScanImagesHFAsync(ScanImagesHFRequest scanImagesHFRequest);
        Task<LambdaAndCombineResponse> LambdaAndCombine(string jsonResult, ThinkMovesChessGame thinkMovesChessGame);
    }
