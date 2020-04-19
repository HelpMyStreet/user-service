using NUnit.Framework;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using UserService.Core.Utils;

namespace UserService.UnitTests
{
    public class HttpContentUtilsTests
    {
        [Test]
        public async Task SerialiseAndCompressContent()
        {
            TestObject testObject = new TestObject()
            {
                Id = 99
            };

            StreamContent result = HttpContentUtils.SerialiseToJsonAndCompress(testObject);

            Stream stream = await result.ReadAsStreamAsync();

            TestObject deserialisedAndDecompressedContent;
            using (GZipStream decompressionStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                deserialisedAndDecompressedContent = await Utf8Json.JsonSerializer.DeserializeAsync<TestObject>(decompressionStream);
            }

            Assert.AreEqual(testObject.Id, deserialisedAndDecompressedContent.Id);
        }
    }

    public class TestObject
    {
        [DataMember(Name = "i")]
        public int Id { get; set; }
    }
}
