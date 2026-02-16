using AutoMapper;
using OrderService.Core.Models.Products;
using OrderService.Domain.Entities.Products;

namespace OrderService.Core.MappingProfile
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            this.CreateMap<Product, ProductDto>().ReverseMap();
        }
    }
}
