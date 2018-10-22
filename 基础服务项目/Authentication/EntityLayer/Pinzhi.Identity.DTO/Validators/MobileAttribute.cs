using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Pinzhi.Identity.Dto.Validators
{
    /// <summary>
    /// 对象不能为空
    /// </summary>
    public class MobileAttribute : ValidationAttribute
    {
        //手机号正则表达式
        private static Regex _mobileregex = new Regex("^(13|14|15|16|17|18|19)[0-9]{9}$");
        private string ErrorCode { get; set; }
        public MobileAttribute(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        private string GetErrorMessage()
        {
            return string.Concat("{", string.Format("\"ErrorCode\":\"{0}\",\"Message\":\"{1}\"", ErrorCode, ErrorMessage), "}");
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errormsg = GetErrorMessage();
            if (value == null)
                return ValidationResult.Success;
            if (string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;
            if (_mobileregex.IsMatch(value.ToString()))
                return ValidationResult.Success;
            else
               return new ValidationResult(errormsg);
        }
    }
}
