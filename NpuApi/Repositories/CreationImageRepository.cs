using Amazon.S3;
using Amazon.S3.Model;

namespace NpuApi.Services
{

    public interface ICreationImageRepository
    {
        Task<string> UploadFileAsync(Stream fileStream, string contentType);
    }
    public class CreationImageRepository : ICreationImageRepository
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        public CreationImageRepository()
        {
            _s3Client = new AmazonS3Client(Amazon.RegionEndpoint.EUWest1);

            // TODO: Inject the bucket name from environment variables or configuration
            _bucketName = "npu--creations";
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string contentType)
        {
            try
            {
                string fileKey = Guid.NewGuid().ToString();

                Console.WriteLine($"Uploading file to S3: {fileKey}");

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey,
                    InputStream = fileStream,
                    ContentType = contentType,
                };


                var response = await _s3Client.PutObjectAsync(putRequest);

                Console.WriteLine($"File uploaded successfully: {fileKey}");

                return GetPresignedUrl(fileKey);
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Error uploading file to S3: {ex.Message}", ex);
            }
        }

        private string GetPresignedUrl(string fileKey)
        {
            try
            {
                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _bucketName,
                    Key = fileKey,
                    Expires = DateTime.UtcNow.AddDays(1),
                    Protocol = Protocol.HTTPS
                };
                return _s3Client.GetPreSignedURL(urlRequest);
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception($"Error generating presigned URL: {ex.Message}", ex);
            }
        }
    }
}