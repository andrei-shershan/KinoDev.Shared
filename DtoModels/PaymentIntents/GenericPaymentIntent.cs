using KinoDev.Shared.Enums;

namespace KinoDev.Shared.DtoModels.PaymentIntents
{
    public class GenericPaymentIntent
    {
        public string PaymentIntentId { get; set; }

        public Guid OrderId { get; set; }

        public string ClientSecret { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }
        
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        public string State { get; set; }

        public PaymentProvider PaymentProvider { get; set; }
    }
}