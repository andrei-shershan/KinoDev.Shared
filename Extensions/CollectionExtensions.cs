namespace KinoDev.Shared.Extensions
{
    public static class CollectionExtensions
    {
        public static bool NullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection == null || !collection.Any();
        }
    }
}