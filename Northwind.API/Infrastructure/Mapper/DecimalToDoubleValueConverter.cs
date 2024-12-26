using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class DecimalToDoubleValueConverter : ITypeConverter<decimal?, DoubleValue>
    {
        public DoubleValue Convert(decimal? source, DoubleValue destination, ResolutionContext context)
        {
            return source.HasValue ? new DoubleValue { Value = (double) source.Value } : null;
        }
    }
}
