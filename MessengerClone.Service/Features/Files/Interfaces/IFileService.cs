using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.Files.Helpers;
using Microsoft.AspNetCore.Http;

namespace MessengerClone.Service.Features.Files.Interfaces
{
    
    public interface IFileService
    {
        Task<Result<string>> SaveAsync(IFormFile file, enFileCategory fileCategory, int id);

        Task<Result<string>> ReplaceAsync(IFormFile newFile, string? existingIFileUrl, enFileCategory fileCategory, int id);

        Result Delete(string relativePath);

        Task<Result> DeleteAsync(string? relativePath);

        Task<Result> DeleteRangeAsync(List<string>? relativePaths);

        Task<Result<List<string>>> ReplaceRangeAsync(List<(IFormFile newFile, string existingFileUrl, int id)> files, enFileCategory fileCategory);
    }
}
