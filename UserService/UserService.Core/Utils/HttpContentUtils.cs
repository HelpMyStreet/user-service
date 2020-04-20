using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;

namespace UserService.Core.Utils
{
    public class HttpContentUtils
    {
        public static StreamContent SerialiseToJsonAndCompress<T>(T content)
        {
            byte[] serialisedBytes = Utf8Json.JsonSerializer.Serialize(content);

            MemoryStream memoryStream = new MemoryStream();
            using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(serialisedBytes, 0, serialisedBytes.Length);
            }

            memoryStream.Position = 0;
            StreamContent streamContent = new StreamContent(memoryStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            streamContent.Headers.ContentEncoding.Add("gzip");

            return streamContent;
        }

    }
}
