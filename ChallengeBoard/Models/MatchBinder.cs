using System;
using System.Web.Mvc;
using System.Linq;

namespace ChallengeBoard.Models
{
    public class MatchBinder : DefaultModelBinder
    {
        private readonly IRepository _repositiory;

        public MatchBinder()
        {
            _repositiory = DependencyResolver.Current.GetService<IRepository>();
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            //ModelStateDictionary mState = bindingContext.ModelState;
            //mState.Add("Property", new ModelState { });
            //mState.AddModelError("Property", "There's an error.");

            var board = _repositiory.GetBoardByIdWithCompetitors(GetValue<int>(bindingContext, "Id"));
            var winner = board.Competitors.SingleOrDefault(x => x.Name == controllerContext.HttpContext.User.Identity.Name);
            var loser = board.Competitors.SingleOrDefault(x => x.Name == GetValue<string>(bindingContext, "Loser"));

            var match = new Match
            {
                Board = board,
                Tied = GetValue<bool>(bindingContext, "Tie"),
                Winner = winner,
                Loser = loser,
                Created = DateTime.Now,
            };

            return (match);
        }

        private T GetValue<T>(ModelBindingContext bindingContext, string key)
        {
            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(key);
            bindingContext.ModelState.SetModelValue(key, valueResult);  
            return (T)valueResult.ConvertTo(typeof(T));
        }
    }
}