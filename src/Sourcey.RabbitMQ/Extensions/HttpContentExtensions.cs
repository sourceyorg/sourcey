using System.Text.Json;

namespace Sourcey.RabbitMQ.Extensions
{
    internal static class HttpContentExtensions
    {
        public static async Task<T?> ReadAsAsync<T>(this HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var value = await content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<T>(value);
        }
    }
}
