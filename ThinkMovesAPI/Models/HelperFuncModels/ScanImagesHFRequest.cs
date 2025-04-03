using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace ThinkMovesAPI.Models.HelperFuncModels;

public class ScanImagesHFRequest
    {
        public List<IFormFile> ScanImagesHFReqVar { get; set; }
    }
