using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Files.Helpers;
using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.General.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;


namespace MessengerClone.Service.Features.Files.Services
{
    public class FileService(IWebHostEnvironment hostEnvironment) : IFileService
    {
        public async Task<Result<string>> SaveAsync(IFormFile file, enFileCategory fileCategory,int id)
        {
            if (!IsValid(file, fileCategory.MediaType()))
                return Result<string>.Failure("File is invalid");

            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{Guid.NewGuid()}{extension}";

            var folderPath = Path.Combine(hostEnvironment.WebRootPath ?? "wwwroot", "uploads", fileCategory.ToString(), id.ToString());
            try
            {
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Result<string>.Success($"/uploads/{fileCategory}/{id}/{fileName}");

            }
            catch (Exception ex)
            {
                // Log error here
                return Result<string>.Failure("Failed to save the file");
            }
        }

        public Result Delete(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return Result.Failure("File path is invalid");

            try
            {
                var filePath = Path.Combine(hostEnvironment.WebRootPath ?? "wwwroot", relativePath.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);

                return Result.Success();
            }
            catch (Exception ex)
            {
                // Log error here
                return Result.Failure("Failed to delete the file");
            }
        }

        public async Task<Result> DeleteAsync(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return Result.Failure("File path is invalid");

            try
            {
                var filePath = Path.Combine(hostEnvironment.WebRootPath ?? "wwwroot", relativePath.TrimStart('/'));
                if (File.Exists(filePath))
                    await Task.Run(() => File.Delete(filePath));

                return Result.Success();

            }
            catch (Exception ex)
            {
                return Result.Failure("Failed to delete the file");
            }
        }

        public async Task<Result> DeleteRangeAsync(List<string>? relativePaths)
        {
            if (relativePaths == null)
                return Result.Failure("Files path are invalid");

                try
                {
                    foreach (var path in relativePaths)
                    {
                        var filePath = Path.Combine(hostEnvironment.WebRootPath ?? "wwwroot", path.TrimStart('/'));
                        if (File.Exists(filePath))
                            await Task.Run(() => File.Delete(filePath));
                    }

                return Result.Success();

            }
            catch (Exception ex)
            {
                // Log error here
                return Result.Failure("Failed to delete the files");
            }
        }

        public async Task<Result<string>> ReplaceAsync(IFormFile newFile, string? existingFileUrl, enFileCategory fileCategory,int id)
        {
            if (!string.IsNullOrWhiteSpace(existingFileUrl))
                await DeleteAsync(existingFileUrl);

            return await SaveAsync(newFile, fileCategory,id);
        }

        public async Task<Result<List<string>>> ReplaceRangeAsync(List<(IFormFile newFile, string existingFileUrl,int id)> files, enFileCategory fileCategory)
        {
            var resultData = new List<string>();
            foreach (var (newFile, url,id) in files)
            {
                var result = await ReplaceAsync(newFile, url, fileCategory,id);
                if(!result.Succeeded)
                    return Result<List<string>>.Failure("Failed to replace the file");

                resultData.Add(result.Data!);
            }

            return Result<List<string>>.Success(resultData);
        }

    
        private bool IsValid(IFormFile file, enMediaType type)
        {
            return ValidationHelper.HasAllowedExtension(file, type) && ValidationHelper.IsWithinAllowedSize(file, type);
        }

    }
}
