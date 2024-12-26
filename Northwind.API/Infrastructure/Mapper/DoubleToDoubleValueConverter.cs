using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class DoubleToDoubleValueConverter : ITypeConverter<double?, DoubleValue>
    {
        public DoubleValue Convert(double? source, DoubleValue destination, ResolutionContext context)
        {
            return source.HasValue ? new DoubleValue { Value = source.Value } : null;
        }
    }
}
