namespace Dake.ViewModel
{
    public class PaymentResponseModel
    {
        public PaymentResponseModel(bool succeeded, string gatewayUrl = null, string error = null)
        {
            Succeeded = succeeded;
            GatewayUrl = gatewayUrl;
            Error = error;
        }

        public bool Succeeded { get; set; }

        public string GatewayUrl { get; set; }

        public string Error { get; set; }
    }
}
