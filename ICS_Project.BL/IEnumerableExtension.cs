using System.Collections.ObjectModel;

namespace ICS_Project.BL;

public static class EnumerableExtension
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> values)
        => new(values);
}