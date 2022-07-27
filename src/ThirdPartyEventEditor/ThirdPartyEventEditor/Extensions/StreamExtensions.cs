using System;
using System.IO;
using System.Threading.Tasks;

namespace ThirdPartyEventEditor.Extensions
{
    public static class StreamExtensions
    {
        public async static Task<string> GetBase64StringAsync(this Stream stream)
        {
            byte[] imageBytes = new byte[stream.Length];

            using(var ms = new MemoryStream(imageBytes))
            {
                await stream.CopyToAsync(ms);
            }

            return Convert.ToBase64String(imageBytes);
        }
    }
}