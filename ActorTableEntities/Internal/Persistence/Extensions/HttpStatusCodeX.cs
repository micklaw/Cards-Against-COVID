namespace ActorTableEntities.Internal.Persistence.Extensions
{
    internal static class HttpStatusCodeX
    {
        public static bool IsSuccess(this int code) => (code >= 200 && code <= 299);
    }
}
