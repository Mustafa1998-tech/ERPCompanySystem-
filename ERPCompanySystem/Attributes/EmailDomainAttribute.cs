using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace ERPCompanySystem.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EmailDomainAttribute : ValidationAttribute
    {
        private readonly string[] _allowedDomains;

        public EmailDomainAttribute(params string[] allowedDomains)
        {
            _allowedDomains = allowedDomains;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            var email = value.ToString();
            if (string.IsNullOrEmpty(email)) return ValidationResult.Success;

            var domain = email.Split('@')[1];
            if (!_allowedDomains.Contains(domain, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult(
                    $"Email must be from one of these domains: {string.Join(", ", _allowedDomains)}");
            }

            return ValidationResult.Success;
        }
    }
}
