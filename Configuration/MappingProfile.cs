using AutoMapper;
using WebApi.Dto;
using WebApi.Models;

namespace WebApi.Configuration
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorResponseDto>();
            CreateMap<AuthorCreateDto, Author>();
            CreateMap<AuthorUpdateDto, Author>();

            CreateMap<Book, BookResponseDto>();
            CreateMap<BookCreateDto, Book>();
            CreateMap<BookUpdateDto, Book>();

            CreateMap<User, UserResponseDto>()
               .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(dest => dest.OrderItemID, opt => opt.MapFrom(src => src.OrderItemId))
                .ForMember(dest => dest.BookID, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BookPrice, opt => opt.MapFrom(src => src.Book.Price));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.Customer.FirstName))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.Customer.LastName))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.StatusID, opt => opt.MapFrom(src => src.StatusId))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.Name));

            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusID))
                .ForMember(dest => dest.OrderDate, opt => opt.Ignore())
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.OrderItems, opt => opt.Ignore());

            CreateMap<OrderItemCreateDto, OrderItem>()
                .ForMember(dest => dest.OrderItemId, opt => opt.Ignore())
                .ForMember(dest => dest.OrderId, opt => opt.Ignore())
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookID))
                .ForMember(dest => dest.Order, opt => opt.Ignore())
                .ForMember(dest => dest.Book, opt => opt.Ignore());
        }
    }
}
