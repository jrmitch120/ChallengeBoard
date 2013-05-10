using System.Collections.Generic;

namespace ChallengeBoard.Infrastucture
{
    public static class CollectionExtensions
    {

        /// <summary>
        /// Adds the elements of the specified enumerable to the end of the ICollection&lt;T&gt;. 
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="destination">Destination Collection</param>
        /// <param name="source">Source enumerable</param>
        public static void AddRange<T>(this ICollection<T> destination,
                                   IEnumerable<T> source)
        {
            foreach (var item in source)
                destination.Add(item);
        }
    }
}