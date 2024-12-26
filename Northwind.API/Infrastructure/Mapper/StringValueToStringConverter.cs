using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class StringValueToStringConverter : ITypeConverter<StringValue, string>
    {
        public string Convert(StringValue source, string destination, ResolutionContext context)
        {
            return source?.Value;
        }
    }
}
