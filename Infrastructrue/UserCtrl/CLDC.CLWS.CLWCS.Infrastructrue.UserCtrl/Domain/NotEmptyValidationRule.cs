using System.Globalization;
using System.Windows.Controls;

namespace CLDC.Infrastructrue.UserCtrl.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value==null)
            {
                return new ValidationResult(false, "值不能为空");
            }

            string stringValue = value.ToString();
            if (stringValue.Length<=2)
            {
                return new ValidationResult(false,"长度必须大于3");
            }
            return ValidationResult.ValidResult;
        }
    }
}
