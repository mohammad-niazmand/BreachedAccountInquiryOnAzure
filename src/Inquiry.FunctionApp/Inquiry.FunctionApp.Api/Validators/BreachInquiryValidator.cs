using Inquiry.Func.Models;
using Newtonsoft.Json;

namespace Inquiry.Func.Validators
{
    public static class BreachInquiryValidator
    {
        public static (bool isValid, string message) Validate(BreachInquiryRequest breachInquiryRequest)
        {
            if (breachInquiryRequest?.Account is null)
                return (false, $"Request body is not in a correct format.\nValid format ex:{JsonConvert.SerializeObject(new BreachInquiryRequest { Account = "Your account name" })} ");

            if (breachInquiryRequest?.Account.Trim().Length == 0)
                return (false, $"{nameof(breachInquiryRequest.Account)} must have a value!");

            return (true, string.Empty);
        }
    }
}
