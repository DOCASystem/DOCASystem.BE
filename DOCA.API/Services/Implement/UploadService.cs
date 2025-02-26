using AutoMapper;
using DOCA.API.Services.Interface;
using DOCA.Domain.Configuration;
using DOCA.Domain.Models;
using DOCA.Repository.Interfaces;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace DOCA.API.Services.Implement;

public class UploadService : BaseService<UploadService>, IUploadService
{
    private readonly AwsSettings _awsSettings;
    public UploadService(IUnitOfWork<DOCADbContext> unitOfWork, 
        ILogger<UploadService> logger, 
        IMapper mapper, IHttpContextAccessor httpContextAccessor, 
        IConfiguration configuration, IOptions<AwsSettings> options) : base(unitOfWork, logger, mapper, httpContextAccessor, configuration)
    {
        _awsSettings = options.Value;
    }


    public async Task<string> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BadHttpRequestException("Không tìm thấy file");
        }

        var allowedExtensions = new[] { ".jpeg", ".png", ".jpg", ".gif", ".bmp", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (!allowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                "Chỉ các định dạng tệp txt, .pdf, .doc, .docx, .xls, .xlsx, .ppt, và .pptx được phép tải lên.");
        try
        {
            var minio = new MinioClient()
                .WithEndpoint(_awsSettings.EndPoint)
                .WithCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey)
                .Build();

            var headers = new Dictionary<string, string>
            {
                { "x-amz-acl", "public-read" }
            };
            var objectName = $"{Guid.NewGuid().ToString()}{extension}";
            var result = await minio.PutObjectAsync(new PutObjectArgs()
                .WithBucket(_awsSettings.BucketName)
                .WithObject(objectName)
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length)
                .WithContentType("image/jpeg")
                .WithHeaders(headers)
            );
            if (result == null)
                throw new MinioException("Failed to upload image");

            return $"https://{_awsSettings.EndPoint}/{_awsSettings.BucketName}/{objectName}";
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to upload image: {e.Message}");
            throw new Exception("Failed to upload image", e);
        }

    }
    
    public async Task<List<string>> UploadImageAsync(List<IFormFile> images)
    {
        if (images == null || !images.Any())
        {
            throw new BadHttpRequestException("Không tìm thấy file nào để tải lên.");
        }

        var uploadedUrls = new List<string>();

        foreach (var image in images)
        {
            try
            {
                var url = await UploadImageAsync(image);
                uploadedUrls.Add(url);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tải lên hình ảnh: {ex.Message}");
                // Optional: Continue uploading even if one file fails
            }
        }

        return uploadedUrls;
    }

}