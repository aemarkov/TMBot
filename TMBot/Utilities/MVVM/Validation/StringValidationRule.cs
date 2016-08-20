using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;

namespace TMBot.Utilities.MVVM.Validation
{
    public class DecimalValidationRule:ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;
            if(str==null)
                return  new ValidationResult(false,"wtf");

            decimal result;
            if(decimal.TryParse(str, out result))
                return new ValidationResult(true, null);
            else
                return new ValidationResult(false, "wtf");
        }
    }

    public class IntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string str = value as string;
            if (str == null)
                return new ValidationResult(false, "wtf");

            int result;
            if (int.TryParse(str, out result))
                return new ValidationResult(true, null);
            else
                return new ValidationResult(false, "wtf");
        }
    }
}