namespace TFAuto.Domain.ExtensionMethods;

public static class TaskExtensions
{
    public static IEnumerable<T> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        return Task.WhenAll(tasks).Result;
    }
}
