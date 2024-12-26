using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using NorthwindDAL.Models;
using NorthwindDAL.Views;

namespace Northwind.API.Infrastructure.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Int32Value, int?>().ConvertUsing<Int32ValueToInt32Converter>();
            CreateMap<int?, Int32Value>().ConvertUsing<Int32ToInt32ValueConverter>();
            CreateMap<StringValue, string>().ConvertUsing<StringValueToStringConverter>();
            CreateMap<string, StringValue>().ConvertUsing<StringToStringValueConverter>();
            CreateMap<decimal?, DoubleValue>().ConvertUsing<DecimalToDoubleValueConverter>();
            CreateMap<DoubleValue, decimal?>().ConvertUsing<DoubleValueToDecimalConverter>();
            CreateMap<DateTime?, Timestamp>().ConvertUsing<DateTimeToTimestampConverter>();
            CreateMap<Timestamp, DateTime?>().ConvertUsing<TimestampToDateTimeConverter>();
            CreateMap<double?, DoubleValue>().ConvertUsing<DoubleToDoubleValueConverter>();
            CreateMap<DoubleValue, double?>().ConvertUsing<DoubleValueToDoubleConverter>();


            CreateMap<OrderWithStatusView, OrderWithStatus>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderID))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate))
                .ForMember(dest => dest.ShippedDate, opt => opt.MapFrom(src => src.ShippedDate))
                .ForMember(dest => dest.ShipVia, opt => opt.MapFrom(src => src.ShipVia))
                .ForMember(dest => dest.Freight, opt => opt.MapFrom(src => src.Freight))
                .ForMember(dest => dest.ShipName, opt => opt.MapFrom(src => src.ShipName))
                .ForMember(dest => dest.ShipAddress, opt => opt.MapFrom(src => src.ShipAddress))
                .ForMember(dest => dest.ShipCity, opt => opt.MapFrom(src => src.ShipCity))
                .ForMember(dest => dest.ShipRegion, opt => opt.MapFrom(src => src.ShipRegion))
                .ForMember(dest => dest.ShipPostalCode, opt => opt.MapFrom(src => src.ShipPostalCode))
                .ForMember(dest => dest.ShipCountry, opt => opt.MapFrom(src => src.ShipCountry))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));
            
            CreateMap<OrderCreate, OrderCreateView>()
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate))
                .ForMember(dest => dest.ShippedDate, opt => opt.MapFrom(src => src.ShippedDate))
                .ForMember(dest => dest.ShipVia, opt => opt.MapFrom(src => src.ShipVia))
                .ForMember(dest => dest.Freight, opt => opt.MapFrom(src => src.Freight))
                .ForMember(dest => dest.ShipName, opt => opt.MapFrom(src => src.ShipName))
                .ForMember(dest => dest.ShipAddress, opt => opt.MapFrom(src => src.ShipAddress))
                .ForMember(dest => dest.ShipCity, opt => opt.MapFrom(src => src.ShipCity))
                .ForMember(dest => dest.ShipRegion, opt => opt.MapFrom(src => src.ShipRegion))
                .ForMember(dest => dest.ShipPostalCode, opt => opt.MapFrom(src => src.ShipPostalCode))
                .ForMember(dest => dest.ShipCountry, opt => opt.MapFrom(src => src.ShipCountry));
            
            CreateMap<OrderDetailToCreateOrder, OrderDetailToCreateOrderView>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));


            CreateMap<gOrder, Order>()
                .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate))
                .ForMember(dest => dest.ShippedDate, opt => opt.MapFrom(src => src.ShippedDate))
                .ForMember(dest => dest.ShipVia, opt => opt.MapFrom(src => src.ShipVia))
                .ForMember(dest => dest.Freight, opt => opt.MapFrom(src => src.Freight))
                .ForMember(dest => dest.ShipName, opt => opt.MapFrom(src => src.ShipName))
                .ForMember(dest => dest.ShipAddress, opt => opt.MapFrom(src => src.ShipAddress))
                .ForMember(dest => dest.ShipCity, opt => opt.MapFrom(src => src.ShipCity))
                .ForMember(dest => dest.ShipRegion, opt => opt.MapFrom(src => src.ShipRegion))
                .ForMember(dest => dest.ShipPostalCode, opt => opt.MapFrom(src => src.ShipPostalCode))
                .ForMember(dest => dest.ShipCountry, opt => opt.MapFrom(src => src.ShipCountry))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<GetAllOrderDetailsView, OrderDetailed>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderID))
                .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
                .ForMember(dest => dest.EmployeeID, opt => opt.MapFrom(src => src.EmployeeID))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.RequiredDate, opt => opt.MapFrom(src => src.RequiredDate))
                .ForMember(dest => dest.ShippedDate, opt => opt.MapFrom(src => src.ShippedDate))
                .ForMember(dest => dest.ShipVia, opt => opt.MapFrom(src => src.ShipVia))
                .ForMember(dest => dest.Freight, opt => opt.MapFrom(src => src.Freight))
                .ForMember(dest => dest.ShipName, opt => opt.MapFrom(src => src.ShipName))
                .ForMember(dest => dest.ShipAddress, opt => opt.MapFrom(src => src.ShipAddress))
                .ForMember(dest => dest.ShipCity, opt => opt.MapFrom(src => src.ShipCity))
                .ForMember(dest => dest.ShipRegion, opt => opt.MapFrom(src => src.ShipRegion))
                .ForMember(dest => dest.ShipPostalCode, opt => opt.MapFrom(src => src.ShipPostalCode))
                .ForMember(dest => dest.ShipCountry, opt => opt.MapFrom(src => src.ShipCountry))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderDetailWithProductInfoView, OrderDetailWithProductInfo>()
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
        }
    }
}
