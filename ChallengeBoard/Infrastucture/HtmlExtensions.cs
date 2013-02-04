using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace ChallengeBoard.Infrastucture
{
    public static class HtmlExtensions
    {
        /// <summary>
        /// MVC extension to display the "DisplayName" of an enumerable class field.
        /// </summary>
        public static MvcHtmlString DisplayNameFor<TModel, TClass, TProperty>(this HtmlHelper<TModel> helper,
                                                                              IEnumerable<TClass> model,
                                                                              Expression<Func<TClass, TProperty>>
                                                                                  expression)
        {
            var name = ExpressionHelper.GetExpressionText(expression);
            name = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(
                () => Activator.CreateInstance<TClass>(), typeof(TClass), name);

            return new MvcHtmlString(metadata.DisplayName);
        }

        public static MvcHtmlString MenuLink(this HtmlHelper helper,
                                       string text,
                                       string action,
                                       string controller,
                                       object routeValues,
                                       object htmlAttributes,
                                       string[] selectedOnControllers = null,
                                       string[] mustHaveParms = null,
                                       string[] mustNotHaveParms = null)
        {
            string menuLink =  ((
                                    controller.ToLower() == helper.ViewContext.RouteData.Values["controller"].ToString().ToLower() &&
                                    action.ToLower() == helper.ViewContext.RouteData.Values["action"].ToString().ToLower()
                                ) || (
                                    selectedOnControllers != null &&
                                    selectedOnControllers.Any(x => x.ToLower() == helper.ViewContext.RouteData.Values["controller"].ToString().ToLower())
                                )) && (
                                    mustHaveParms == null ||
                                    helper.ViewContext.HttpContext.Request.QueryString
                                    .AllKeys
                                    .Intersect(mustHaveParms, StringComparer.InvariantCultureIgnoreCase)
                                    .Count() == mustHaveParms.Count()
                                ) && (
                                    mustNotHaveParms == null ||
                                    !helper.ViewContext.HttpContext.Request.QueryString
                                    .AllKeys
                                    .Intersect(mustNotHaveParms, StringComparer.InvariantCultureIgnoreCase)
                                    .Any()
                                )
                               ? "<li class=\"active\">" : "<li>";

            menuLink += helper.ActionLink(text, action, controller, new RouteValueDictionary(routeValues), new RouteValueDictionary(htmlAttributes));
            menuLink += "</li>";
            return (MvcHtmlString.Create(menuLink));
        }
    }
}