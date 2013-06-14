using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ChallengeBoard.Models.Validation
{
    public sealed class DateWithinYear : RangeAttribute, IClientValidatable
    {
        public DateWithinYear()
            : base(typeof(DateTime), DateTime.Now.ToShortDateString(), DateTime.Now.AddYears(1).ToShortDateString()) { }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "isdatewithinyear"
            };
            
            yield return rule;
        }
    }
}