using Dake.Models;
using Dake.ViewModel;
using System.Threading.Tasks;

namespace Dake.Service.Interface
{
    public interface IPaymentService
    {
        Task AddPaymentAttempt(PaymentRequestAttemp attempt);

        Task<PaymentResponseModel> ConnectGateway(PaymentConnectModel model);

        Task<(bool Succeeded, long? RefId)> VerifyPayment(string authority, int amount);
    }
}
