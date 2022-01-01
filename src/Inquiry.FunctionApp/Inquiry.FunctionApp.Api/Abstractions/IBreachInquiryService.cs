using Inquiry.Func.Models;
using System.Threading.Tasks;

namespace Inquiry.Func.Abstractions
{
    public interface IBreachInquiryService
    {
        Task<(bool isBreached, BreachedInfo breachedInfo)> ValidateAccountAsync(string accountName);
    }
}
