using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class DoubleValueToDoubleConverter : ITypeConverter<DoubleValue, double?>
    {
        public double? Convert(DoubleValue source, double? destination, ResolutionContext context)
        {
            return source?.Value;
        }
    }
}
