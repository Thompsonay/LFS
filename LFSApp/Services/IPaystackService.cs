using System.Threading.Tasks;

namespace LFSApp.Services
{
    public interface IPaystackService
    {
        Task<string> InitializeTransactionAsync(string email, decimal amount, string reference, string callbackUrl);
        Task<bool> VerifyTransactionAsync(string reference);
    }
}
