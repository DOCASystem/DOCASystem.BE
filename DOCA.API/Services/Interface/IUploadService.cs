namespace DOCA.API.Services.Interface;

public interface IUploadService
{ 
    Task<string> UploadImageAsync(IFormFile file);
    Task<List<string>> UploadImageAsync(List<IFormFile> images);
}