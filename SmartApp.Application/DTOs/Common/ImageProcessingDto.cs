using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.DTOs.Common
{
    public class FileImageSettings
    {
        public string BaseFolderPath_For_Images { get; set; }
        public string BaseFolderPath_For_Files { get; set; }
        public int MaxImageFileSizeInMB { get; set; } = 2;
        public int MaxDocumentFileSizeInMB { get; set; } = 5;
        public string[] AllowedImageMimeTypes { get; set; }
        public string[] AllowedDocumentMimeTypes { get; set; }
    }


    public class ImageProcessedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public string Base64Image { get; set; }
        public byte[] ByteImage { get; set; }
    }

    public class FileProcessedResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
