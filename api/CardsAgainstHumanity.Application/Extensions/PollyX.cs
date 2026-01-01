using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;

namespace CardsAgainstHumanity.Application.Extensions
{
    public static class PollyX
    {
        public static async Task<T> WithRetry<TClient, T>(this TClient client, Func<TClient, Task<T>> method, int attempts = 4, double backOffFactor = 1.0)
        {
            var response = await Policy
                .HandleResult<T>(r => r == null)
                .Or<Exception>()
                .WaitAndRetryAsync(attempts, count => TimeSpan.FromMilliseconds(count * backOffFactor))
                .ExecuteAndCaptureAsync(() => method(client));

            return response.Result;
        }
    }
}
