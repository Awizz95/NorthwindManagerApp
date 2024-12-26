using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class DoubleValueToDecimalConverter : ITypeConverter<DoubleValue, decimal?>
    {
        public decimal? Convert(DoubleValue source, decimal? destination, ResolutionContext context)
        {
            return (decimal) source?.Value;
        }
    }
}
