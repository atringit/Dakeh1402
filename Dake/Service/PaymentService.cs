using Dake.DAL;
using Dake.Models;
using Dake.Service.Interface;
using Dake.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly Context _context;
        private readonly ILogger<PaymentService> _logger;
        private readonly string _zarinPalKey;
        private readonly string _zarinPalGateway;

        public PaymentService(IConfiguration configuration, Context context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
            _zarinPalKey = configuration.GetSection("ZarinPalKey").Value;
            _zarinPalGateway = configuration.GetSection("ZarinPalGateway").Value;
        }

        public async Task AddPaymentAttempt(PaymentRequestAttemp attempt)
        {
            _context.Add(attempt);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentResponseModel> ConnectGateway(PaymentConnectModel model)
        {
            var request = new Zarinpal.Payment(merchantId: _zarinPalKey, amount: model.Amount);

            try
            {
                var response = await request.PaymentRequest(
                    description: $"پرداخت فاکتور شمارهی {model.FactorId}",
                    callbackUrl: model.ReturnUrl,
                    mobile: model.UserMobile);

                if (response?.Status == 100)
                {
                    var gatewayUrl = _zarinPalGateway + response.Authority;

                    return new PaymentResponseModel(
                        succeeded: true,
                        gatewayUrl: gatewayUrl);
                }

                return new PaymentResponseModel(
                    succeeded: false,
                    error: "امکان اتصال به درگاه بانکی وجود ندارد");
            }
            catch (Exception ex)
            {
                _logger.LogError("PaymentError | {0}", ex.Message);

                return new PaymentResponseModel(
                    succeeded: false,
                    error: "امکان اتصال به درگاه بانکی وجود ندارد");
            }
        }

        public async Task<(bool Succeeded, long? RefId)> VerifyPayment(string authority, int amount)
        {
            var request = new Zarinpal.Payment(merchantId: _zarinPalKey, amount: amount);

            var response = await request.Verification(authority);

            return (response?.Status == 100, response?.RefId);
        }
    }
}
