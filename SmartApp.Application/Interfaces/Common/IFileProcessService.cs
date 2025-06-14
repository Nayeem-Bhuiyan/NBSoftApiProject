using Microsoft.AspNetCore.Http;
using SmartApp.Application.DTOs.Common;
using SmartApp.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.Interfaces.Common
{
    public interface IFileProcessService
    {
        Task<ImageProcessedResult> ProcessImageAsync(IFormFile imageFile, string fileName = null, bool willSaveInFolder = false, string subfolder = null, CancellationToken cancellationToken = default);
        Task<FileProcessedResult> SaveDocumentAsync(IFormFile docFile, string fileName = null, string subfolder = null, CancellationToken cancellationToken = default);
        string DeleteImage(string imageName);
        CommonResponse RenameImage(string currentName, string newName);
        string GetImageFullPath(string imageName);
        string GetDocumentFullPath(string docName);
    }
}
