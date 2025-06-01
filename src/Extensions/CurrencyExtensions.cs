using KinoDev.Shared.Enums;

namespace KinoDev.Shared.Extensions
{
    public static class CurrencyExtensions
    {
        public static string StringValue(this Currency currency)
        {
            return currency.ToString().ToLower();
        }
    }
}