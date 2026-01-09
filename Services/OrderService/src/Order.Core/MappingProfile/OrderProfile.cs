using AutoMapper;
using OrderService.Core.Models.Orders;
using OrderService.Domain.Entities.Oriders;

namespace OrderService.Core.MappingProfile
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            this.CreateMap<Order, OrderDto>().ReverseMap();
            this.CreateMap<Order, OrderCreateDto>().ReverseMap();
            this.CreateMap<Order, OrderUpdateDto>().ReverseMap();
        }
    }
}
