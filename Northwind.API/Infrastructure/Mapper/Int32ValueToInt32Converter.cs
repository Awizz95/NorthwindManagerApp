using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class Int32ValueToInt32Converter : ITypeConverter<Int32Value, int?>
    {
        public int? Convert(Int32Value source, int? destination, ResolutionContext context)
        {
            return source?.Value;
        }
    }
}
