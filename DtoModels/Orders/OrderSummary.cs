using KinoDev.Shared.DtoModels.ShowTimes;
using KinoDev.Shared.DtoModels.Tickets;
using KinoDev.Shared.Enums;

namespace KinoDev.Shared.DtoModels.Orders
{
    public class OrderSummary
    {
        public Guid Id { get; set; }

        public decimal Cost { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ShowTimeSummary ShowTimeSummary { get; set; }

        public ICollection<TickerSummary> Tickets { get; set; }
    }
}