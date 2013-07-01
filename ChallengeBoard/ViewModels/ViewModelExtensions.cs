using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;

namespace ChallengeBoard.ViewModels
{
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Map a paged list of models to a viewmodel equivalent
        /// </summary>
        /// <typeparam name="T">Model item type</typeparam>
        /// <typeparam name="TReturn">Viewmodel item type</typeparam>
        /// <param name="baseList"></param>
        /// <param name="itemMapper">Function to map a model item to it's viewmodel</param>
        /// <returns>Paged list containing viewmodels</returns>
        public static IPagedList<TReturn> MapToViewModel<T, TReturn>(this IPagedList<T> baseList,Func<T, TReturn> itemMapper)
        {
            var mappedItems = baseList.Select(itemMapper);
            var newList = new StaticPagedList<TReturn>(mappedItems, baseList.GetMetaData());
            return (newList);
        }
    }
}