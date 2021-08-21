using System.ComponentModel.DataAnnotations;

namespace Klika.Identity.Model.Extensions.Annotations
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private RequiredAttribute _innerAttribute;
        private string _dependentProperty { get; }
        private object _targetValue { get; }

        public RequiredIfAttribute(string dependentProperty, object targetValue)
        {
            _innerAttribute = new RequiredAttribute();
            _dependentProperty = dependentProperty;
            _targetValue = targetValue;
        }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var field = validationContext.ObjectType.GetProperty(_dependentProperty);
            
            if (field != null)
            {
                var dependentValue = field.GetValue(validationContext.ObjectInstance, null);
                if ((dependentValue == null && _targetValue == null) || (dependentValue.Equals(_targetValue)))
                {
                    if (!_innerAttribute.IsValid(value))
                    {
                        return new ValidationResult(ErrorMessage=validationContext.DisplayName + " Is required.");
                    }
                }
                
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(_dependentProperty));
        }
    }
}