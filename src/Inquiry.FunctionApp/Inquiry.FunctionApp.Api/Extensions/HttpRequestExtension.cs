using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Inquiry.Func.Extensions
{
    public static class HttpRequestExtension
    {
        /// <summary>
        ///  Asynchronously reads the UTF-8 encoded text representing a single JSON value
        ///  into an instance of a type specified by a generic type parameter. The stream
        ///   will be read to completion.
        /// </summary>
        /// <typeparam name="T">The target type of the JSON value.</typeparam>
        /// <param name="request"></param>
        /// <param name="cancellationToken">A token that may be used to cancel the read operation.</param>
        /// <returns>A TValue representation of the JSON value.</returns>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpRequest request, CancellationToken cancellationToken) where T : class
        {
            return await JsonSerializer.DeserializeAsync<T>(request.Body, cancellationToken: cancellationToken);
        }
    }
}
