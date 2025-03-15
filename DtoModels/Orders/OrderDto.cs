using KinoDev.Shared.DtoModels.Tickets;
using KinoDev.Shared.Enums;

namespace KinoDev.Shared.DtoModels.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }

        public decimal Cost { get; set; }

        public OrderState State { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public ICollection<TicketDto> Ticket { get; set; } = new List<TicketDto>();
    }
}