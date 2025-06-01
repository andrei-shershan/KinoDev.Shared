using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KinoDev.Shared.Extensions
{
    public static class GenericResponseExtension
    {
        public static async Task<T?> GetResponseAsync<T>(this HttpResponseMessage response, ILogger? logger = null) where T : class
        {
            if (response == null)
            {
                logger?.LogWarning("Response is null");
                return null;
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(content);
                }
                catch (Exception e)
                {
                    logger?.LogError(e, "Error deserializing response content to type {Type}", typeof(T).Name);
                    return null;
                }
            }

            return null;
        }
    }
}