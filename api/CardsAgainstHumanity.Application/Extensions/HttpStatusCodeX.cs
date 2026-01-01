using System.Net;

namespace CardsAgainstHumanity.Application.Extensions
{
    public static class HttpStatusCodeX
    {
        public static bool IsSuccess(this HttpStatusCode code) => IsSuccess((int)code);

        public static bool IsSuccess(this int code) => (code >= 200 && code <= 299);
    }
}
