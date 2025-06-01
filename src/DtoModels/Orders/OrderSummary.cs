using KinoDev.Shared.DtoModels.ShowTimes;
using KinoDev.Shared.DtoModels.Tickets;
using KinoDev.Shared.Enums;

namespace KinoDev.Shared.DtoModels.Orders
{
    public class OrderSummary
    {
        public Guid Id { get; set; }

        public OrderState State { get; set; }

        public decimal Cost { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Guid? UserId { get; set; }

        public string Email { get; set; }

        public bool EmailSent { get; set; }

        public string FileUrl { get; set; }

        public ShowTimeSummary ShowTimeSummary { get; set; }

        public ICollection<TickerSummary> Tickets { get; set; }
    }
}