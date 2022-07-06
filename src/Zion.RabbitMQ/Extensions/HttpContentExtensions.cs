using Utf8Json;

namespace Zion.RabbitMQ.Extensions
{
    internal static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            var value = await content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(value);
        }
    }
}
