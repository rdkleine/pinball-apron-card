namespace PinballApronCard.Helpers;

public static class ListHelper
{
    public static List<T> TakeRange<T>(this List<T> list, int count)
    {
        var result = list.GetRange(0, Math.Min(list.Count, count));
        list.RemoveRange(0, Math.Min(list.Count, count));
        return result;
    }
}