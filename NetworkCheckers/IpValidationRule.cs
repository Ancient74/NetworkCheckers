using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NetworkCheckers
{
    class IpValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return new ValidationResult(IPAddress.TryParse(value?.ToString(), out _), null);
        }
    }
}
