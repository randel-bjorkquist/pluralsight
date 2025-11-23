namespace DAL.Core.Utilities;

public static class EnumerableExtensions
{
  #region Join Method(s)

  /// <summary>
  /// Joins elements of an <see cref="IEnumerable{T}"/> into a string using comma separator.
  /// Fluent alternative to <see cref="string.Join(string, IEnumerable{T})"/>.
  /// </summary>
  /// <typeparam name="T">Type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to join.</param>
  /// <returns>Joined string, or <c>null</c> if <paramref name="source"/> is <c>null</c>.</returns>
  public static string? Join<T>(this IEnumerable<T>? source)
      => source?.Join<T>(',');
      
  /// <summary>
  /// <para>TODO: Fluent extension for joining <see cref="IEnumerable{T}"/> elements with a single-character separator.</para>
  /// <para>This method is commented out pending .NET runtime support for <c>string.Join(char, IEnumerable{T})</c> (introduced in .NET Core 2.1).</para>
  /// <para>Once upgraded (e.g., to .NET 8+), uncomment and use as a null-safe alternative to <see cref="string.Join(string, IEnumerable{T})"/>.</para>
  /// </summary>
  /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> containing the elements to join.</param>
  /// <param name="separator">The single character to use as a separator between each element.</param>
  /// <returns>A string containing all elements from the source sequence, separated by the specified <paramref name="separator"/>. Returns <c>null</c> if <paramref name="source"/> is <c>null</c>.</returns>
  public static string? Join<T>(this IEnumerable<T> source, char separator)
      => source is null ? null
                        : string.Join(separator, source);

  /// <summary>
  /// Joins elements of an <see cref="IEnumerable{T}"/> into a string using the specified separator.
  /// Fluent alternative to <see cref="string.Join(string, IEnumerable{T})"/>.
  /// </summary>
  /// <typeparam name="T">Type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to join.</param>
  /// <param name="separator">Separator string.</param>
  /// <returns>Joined string, or <c>null</c> if <paramref name="source"/> is <c>null</c>.</returns>
  public static string? Join<T>(this IEnumerable<T>? source, string separator)
      => source is null ? null
                        : string.Join(separator, source);

  #endregion

  #region HasItems, IsNullOrEmpty, OrEmptyIfNull Method(s)

  /// <summary>
  /// Determines whether the <see cref="IEnumerable{T}"/> contains any elements.
  /// Equivalent to <c>!source.IsNullOrEmpty()</c> or <c>source.Any()</c>, with null-safety.
  /// </summary>
  /// <typeparam name="T">The type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to check.</param>
  /// <returns><c>true</c> if the source has items; otherwise, <c>false</c>.</returns>
  public static bool HasItems<T>(this IEnumerable<T>? source) 
      => source?.Any() ?? false;

  /// <summary>
  /// Determines whether the <see cref="IEnumerable{T}"/> is null or contains no elements.
  /// Equivalent to <c>string.IsNullOrEmpty</c> for collections.
  /// </summary>
  /// <typeparam name="T">The type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to check.</param>
  /// <returns><c>true</c> if the source is null or empty; otherwise, <c>false</c>.</returns>
  public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
      => source?.Any() != true;

  /// <summary>
  /// Determines whether the <see cref="IEnumerable{T}"/> is null or contains at least one element that satisfies the specified condition.
  /// Returns <c>true</c> if the source is <c>null</c> or any element passes the test; otherwise, <c>false</c>.
  /// </summary>
  /// <typeparam name="T">The type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to check.</param>
  /// <param name="predicate">A function to test each element for a condition.</param>
  /// <returns><c>true</c> if source is null or any element matches; otherwise, <c>false</c>.</returns>
  public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source, Func<T, bool> predicate)
      => !source?.Any(predicate) ?? true;

  /// <summary>
  /// Returns the source <see cref="IEnumerable{T}"/> if not null; otherwise, an empty enumerable.
  /// Avoids null-reference exceptions during enumeration.
  /// </summary>
  /// <typeparam name="T">The type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to return or replace with empty.</param>
  /// <returns>The source if non-null; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
  public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T>? source)
      => source ?? [];

  #endregion

  #region Random Method(s)

  /// <summary>
  /// Returns a random selection of unique elements from the <see cref="IEnumerable{T}"/> (full shuffle if unspecified count).
  /// </summary>
  /// <typeparam name="T">The type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to randomize.</param>
  /// <returns>A randomized enumerable, or empty if source is null/empty.</returns>
  public static IEnumerable<T> Random<T>(this IEnumerable<T>? source)
      => source.Random(int.MaxValue);

  /// <summary>
  /// Returns up to the specified number of unique random elements from the <see cref="IEnumerable{T}"/> (without replacement).
  /// If <paramref name="count"/> exceeds source size, returns a full shuffled version.
  /// </summary>
  /// <typeparam name="T">The type of elements in the source.</typeparam>
  /// <param name="source">The <see cref="IEnumerable{T}"/> to select from.</param>
  /// <param name="count">Maximum number of elements to return (clamped to source size).</param>
  /// <returns>A randomized enumerable, or empty if source is null/empty.</returns>
  public static IEnumerable<T> Random<T>(this IEnumerable<T>? source, int count)
  {
      if (source?.IsNullOrEmpty() ?? true)
          yield break;

      var list = source.ToList();
      count = Math.Min(count, list.Count);

      // Fisher-Yates shuffle for efficiency (O(n))
      var indices = Enumerable.Range(0, list.Count).ToArray();
      var random  = new Random();
          
      for (int i = indices.Length - 1; i > 0; i--)
      {
          int j = random.Next(0, i + 1);
          (indices[i], indices[j]) = (indices[j], indices[i]);
      }

      foreach (int idx in indices.Take(count))
          yield return list[idx];
  }

  #endregion
}
