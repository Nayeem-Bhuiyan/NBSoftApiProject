using SmartApp.Application.DTOs.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SmartApp.Application.Interfaces.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using SmartApp.Shared.Common;



namespace SmartApp.Infrastructure.Services.Common
{

public class FileProcessService : IFileProcessService
    {
        private readonly string _imageUploadFolderDirectory;
        private readonly string _fileUploadFolderDirectory;
        private readonly int _maxImageFileSizeInBytes;
        private readonly int _maxDocBytes;

        private readonly string[] _allowedImageTypes;
        private readonly string[] _allowedDocTypes;


        private readonly IHostEnvironment _env;
        private readonly ILogger<FileProcessService> _logger;
        public FileProcessService(IOptions<FileImageSettings> options, IHostEnvironment env, ILogger<FileProcessService> logger)
        {
            _env = env;
            _logger=logger;

            // Setup paths and limits based on config or defaults
            var settings = options.Value;
            _maxImageFileSizeInBytes =(settings.MaxImageFileSizeInMB > 0 ? settings.MaxImageFileSizeInMB : 5) * 1024 * 1024;
            _maxDocBytes = (settings.MaxDocumentFileSizeInMB > 0 ? settings.MaxDocumentFileSizeInMB : 5) * 1024 * 1024;

            // Allowed MIME types (fallback to defaults)
            _allowedImageTypes = settings.AllowedImageMimeTypes ?? AllowedMimeTypes.ImageTypes;
            _allowedDocTypes = settings.AllowedDocumentMimeTypes ?? AllowedMimeTypes.DocumentTypes;

            var relativeFolderDirectory_for_Images = options.Value.BaseFolderPath_For_Images;
            var relativeFolderDirectory_for_Files = options.Value.BaseFolderPath_For_Files;

            // Build absolute folder paths to store images and docs
            _imageUploadFolderDirectory = Path.Combine(_env.ContentRootPath, relativeFolderDirectory_for_Images);
            _fileUploadFolderDirectory = Path.Combine(_env.ContentRootPath, relativeFolderDirectory_for_Files);

            // Ensure folders exist
            //if (!Directory.Exists(_imageUploadFolderDirectory))
            Directory.CreateDirectory(_imageUploadFolderDirectory);

            //if (!Directory.Exists(_fileUploadFolderDirectory))
            Directory.CreateDirectory(_fileUploadFolderDirectory);
        }

        #region Helper_method
        //private string EnsureFileNameWithExtension(string fileName, string originalFileName)
        //{
        //    var extension = Path.GetExtension(originalFileName);
        //    return !string.IsNullOrWhiteSpace(fileName) && !Path.HasExtension(fileName)
        //        ? fileName + extension
        //        : fileName ?? Guid.NewGuid().ToString() + extension;
        //}

        private string EnsureFileNameWithExtension(string fileName, string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var name = !string.IsNullOrWhiteSpace(fileName)
                ? SanitizeFileName(fileName)
                : Guid.NewGuid().ToString();

            if (!Path.HasExtension(name))
                name += extension;

            return name;
        }

        private string SanitizeFileName(string fileName)
        {
            return string.Concat(fileName.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
        }


        private bool ValidateDocument(IFormFile docFile, out string errorMessage)
        {
            errorMessage = string.Empty;
            var contentType = docFile.ContentType?.Trim().ToLowerInvariant();

            if (docFile == null || docFile.Length == 0)
            {
                errorMessage = "Invalid document file";
                _logger?.LogWarning("Invalid document file");
                return false;
            }

            if (!_allowedDocTypes.Contains(contentType))
            {
                errorMessage = "Unsupported document type.";
                _logger?.LogWarning("Unsupported document type: {ContentType}, allowed: {@Allowed}", contentType, _allowedDocTypes);
                return false;
            }

            if (docFile.Length > _maxDocBytes)
            {
                errorMessage = $"Document size exceeds {_maxDocBytes / (1024 * 1024)} MB limit.";
                _logger?.LogWarning("File size exceeded: {FileSize} bytes", docFile.Length);
                return false;
            }


            return true;
        }

        private bool ValidateImage(IFormFile imageFile, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (imageFile == null || imageFile.Length == 0)
            {
                errorMessage = "Invalid image file.";
                _logger?.LogWarning("Invalid image file");
                return false;
            }

            var contentType = imageFile.ContentType?.Trim().ToLowerInvariant();

            if (!_allowedImageTypes.Contains(contentType))
            {
                errorMessage = "Unsupported image type.";
                _logger?.LogWarning("Unsupported image type: {ContentType}, only allowed types: {AllowedTypes}", contentType, string.Join(", ", _allowedImageTypes));
                return false;
            }

            if (imageFile.Length > _maxImageFileSizeInBytes)
            {
                errorMessage = $"Image size exceeds the {_maxImageFileSizeInBytes / (1024 * 1024)}MB limit.";
                _logger?.LogWarning("File size exceeds the limit: {FileSize} bytes", imageFile.Length);
                return false;
            }

            return true;
        }


        #endregion


        public async Task<ImageProcessedResult> ProcessImageAsync(IFormFile imageFile, string fileName = null, bool willSaveInFolder = false, string subfolder = null, CancellationToken cancellationToken = default)
        {
            var result = new ImageProcessedResult();
            try
            {

                if (!ValidateImage(imageFile, out string errorMessage))
                {
                    result.Success = false;
                    result.Message = errorMessage;
                    return result;
                }

                #region Convert_Image_as_Byte_Array
                using var ms = new MemoryStream();
                await imageFile.CopyToAsync(ms);
                byte[] imageBytes = ms.ToArray();
                #endregion

                string imageNameWithExtention = EnsureFileNameWithExtension(fileName, imageFile.FileName);


                #region Save_Image_In_Folder
                if (willSaveInFolder)
                {
                    //string fullPath = Path.Combine(_imageUploadFolderDirectory, imageNameWithExtention);
                    string fullImagePath = GetFullPath(_imageUploadFolderDirectory, imageNameWithExtention, subfolder);

                    //await File.WriteAllBytesAsync(fullPath, imageBytes,cancellationToken); //process fully use of memory

                    using (var stream = new FileStream(fullImagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream, cancellationToken);

                    }
                }
                #endregion

                #region Response_Result
                if (imageBytes.Length > 0)
                {
                    result.Success = true;
                    result.ImageName = imageNameWithExtention;
                    result.ByteImage = imageBytes;
                    result.Base64Image = Convert.ToBase64String(imageBytes);
                    result.ContentType = imageFile.ContentType;
                    result.Message = "Image saved and byte array generated successfully.";
                }
                else
                {
                    result.Success = false;
                    result.Message = "error while processing image";

                }
                _logger?.LogInformation("Image {ImageName} processed successfully.", imageNameWithExtention);
                #endregion

            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error while processing image.");
                result.Success = false;
                result.Message = $"Error: {ex.Message}";
            }

            return result;
        }


        public async Task<FileProcessedResult> SaveDocumentAsync(IFormFile docFile, string fileName = null, string subfolder = null, CancellationToken cancellationToken = default)
        {
            var result = new FileProcessedResult();

            try
            {

                if (!ValidateDocument(docFile, out string errorMessage))
                {
                    result.Success = false;
                    result.Message = errorMessage;
                    return result;
                }

                string fileNameWithExtention = EnsureFileNameWithExtension(fileName, docFile.FileName);

                //string path = Path.Combine(_fileUploadFolderDirectory, fileNameWithExtention);
                //string path = GetFullPath(_fileUploadFolderDirectory, fileNameWithExtention, "Employee/123"); example 
                string fullFilePath = GetFullPath(_fileUploadFolderDirectory, fileNameWithExtention, subfolder);


                using var stream = new FileStream(fullFilePath, FileMode.Create);
                await docFile.CopyToAsync(stream, cancellationToken);

                result.Success = true;
                result.Message = "Document saved successfully.";
                result.FileName = fileNameWithExtention;
                result.ContentType = docFile.ContentType;

                _logger.LogInformation("Document {FileName} saved successfully.", fileNameWithExtention);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving document.");
                result.Success = false;
                result.Message = $"Error saving document: {ex.Message}";
            }

            return result;
        }


        public string DeleteImage(string imageName)
        {
            try
            {
                string fullPath = Path.Combine(_imageUploadFolderDirectory, imageName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger?.LogInformation("Deleted image {ImageName}.", imageName);
                    return "success";
                }
                return "file not found";
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to delete image {ImageName}.", imageName);
                return "Failed to delete image {ImageName}.";
            }
            
        }

        public CommonResponse RenameImage(string currentName, string newName)
        {
            var data = new CommonResponse();

            try
            {
                var currentPath = Path.Combine(_imageUploadFolderDirectory, currentName);
                var newPath = Path.Combine(_imageUploadFolderDirectory, newName);

                if (!File.Exists(currentPath))
                {
                    data.IsSuccess=false;
                    data.Message=string.Format("Rename failed. File {CurrentName} not found.", currentName);
                    return data;
                }

                if (File.Exists(newPath))
                {
                    data.IsSuccess=false;
                    data.Message=string.Format("Rename failed. Destination file {NewName} already exists.", newName);
                    return data;
                }

                File.Move(currentPath, newPath);
                data.IsSuccess=true;
                data.Message=string.Format("Renamed image from {CurrentName} to {NewName}.", currentName, newName);
                return data;
            }
            catch (Exception ex)
            {
                data.IsSuccess=false;
                data.Message=string.Format("Failed to rename image from {CurrentName} to {NewName}."+ex.Message, currentName, newName);
                return data;
            }
        }


        public bool DeleteDocument(string fileName)
        {
            try
            {
                var fullPath = Path.Combine(_fileUploadFolderDirectory, fileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger?.LogInformation("Deleted document {FileName}.", fileName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to delete document {FileName}.", fileName);
            }
            return false;
        }

        public bool RenameDocument(string currentName, string newName)
        {
            try
            {
                var currentPath = Path.Combine(_fileUploadFolderDirectory, currentName);
                var newPath = Path.Combine(_fileUploadFolderDirectory, newName);

                if (!File.Exists(currentPath))
                {
                    _logger.LogWarning("Rename failed. File {CurrentName} not found.", currentName);
                    return false;
                }

                if (File.Exists(newPath))
                {
                    _logger.LogWarning("Rename failed. Destination file {NewName} already exists.", newName);
                    return false;
                }

                File.Move(currentPath, newPath);
                _logger.LogInformation("Renamed document from {CurrentName} to {NewName}.", currentName, newName);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rename document from {CurrentName} to {NewName}.", currentName, newName);
                return false;
            }
        }



        public string GetImageFullPath(string imageName) =>
          Path.Combine(_imageUploadFolderDirectory, imageName);

        public string GetDocumentFullPath(string docName) =>
            Path.Combine(_fileUploadFolderDirectory, docName);

        private string GetFullPath(string baseDir, string fileName, string subfolder = null)
        {
            string folderPath = !string.IsNullOrEmpty(subfolder)
                ? Path.Combine(baseDir, subfolder)
                : baseDir;

            //if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

            return Path.Combine(folderPath, fileName);
        }

    }




    #region AllowedMimeTypes
    public static class AllowedMimeTypes
    {
        public static readonly string[] ImageTypes = new[]
        {
        "image/jpeg",
        "image/png",
        "image/jpg",
        "image/gif",
        "image/bmp",
        "image/webp",
        "image/tiff",
        "image/x-icon",
        "image/svg+xml"
    };

        public static readonly string[] DocumentTypes = new[]
        {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation"
        };
    }

    #endregion

}
