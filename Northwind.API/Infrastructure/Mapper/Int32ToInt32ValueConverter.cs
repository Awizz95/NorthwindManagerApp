using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class Int32ToInt32ValueConverter : ITypeConverter<int?, Int32Value>
    {
        public Int32Value Convert(int? source, Int32Value destination, ResolutionContext context)
        {
            return source.HasValue ? new Int32Value { Value = source.Value } : null;
        }
    }
}
