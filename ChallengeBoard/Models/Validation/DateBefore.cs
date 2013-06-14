using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ChallengeBoard.Models.Validation
{
    public sealed class DateBefore : ValidationAttribute, IClientValidatable
    {
        private readonly string _testedPropertyName;
        private readonly bool _allowEqualDates;

        public DateBefore(string testedPropertyName, bool allowEqualDates = false)
        {
            _testedPropertyName = testedPropertyName;
            _allowEqualDates = allowEqualDates;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(_testedPropertyName);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult(string.Format("unknown property {0}", _testedPropertyName));
            }

            var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

            if (value == null || !(value is DateTime))
            {
                return ValidationResult.Success;
            }

            if (propertyTestedValue == null || !(propertyTestedValue is DateTime))
            {
                return ValidationResult.Success;
            }

            // Compare values
            if ((DateTime)value <= (DateTime)propertyTestedValue)
            {
                if (_allowEqualDates)
                {
                    return ValidationResult.Success;
                }
                if ((DateTime)value < (DateTime)propertyTestedValue)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
                                                                               ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessageString,
                ValidationType = "isdatebefore"
            };
            rule.ValidationParameters["propertytested"] = _testedPropertyName;
            rule.ValidationParameters["allowequaldates"] = _allowEqualDates;
            yield return rule;
        }
    }
}