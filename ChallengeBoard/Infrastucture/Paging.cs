using System.Linq;

namespace ChallengeBoard.Infrastucture
{
    public static class Paging
    {
        public static IQueryable<T> Paged<T>(this IQueryable<T> source, int page, int pageSize)
        {
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}