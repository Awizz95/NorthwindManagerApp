using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class StringToStringValueConverter : ITypeConverter<string, StringValue>
    {
        public StringValue Convert(string source, StringValue destination, ResolutionContext context)
        {
            return string.IsNullOrEmpty(source) ? null : new StringValue { Value = source };
        }
    }
}
