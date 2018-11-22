using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Pinzhi.Platform.Dto
{
    /// <summary>
    /// 对象不能为空
    /// </summary>
    public class MobileAttribute : ValidationAttribute
    {
        //手机号正则表达式
        private static Regex _mobileregex = new Regex("^(13|14|15|17|18)[0-9]{9}$");
        private string ErrorCode { get; set; }
        public MobileAttribute(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        private string GetErrorMessage()
        {
            return string.Concat("{", string.Format("\"ErrorCode\":\"{0}\",\"ErrorMessage\":\"{1}\"", ErrorCode, ErrorMessage), "}");
        }
        /// <summary>
        /// 是否为手机号
        /// </summary>
        private static bool IsMobile(string s)
        {
            if (string.IsNullOrEmpty(s))
                return true;
            return _mobileregex.IsMatch(s);
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errormsg = GetErrorMessage();
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;
            if (!_mobileregex.IsMatch(value.ToString()))
                return new ValidationResult(errormsg);
            return ValidationResult.Success;
        }
    }
}
