using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class TimestampToDateTimeConverter : ITypeConverter<Timestamp, DateTime?>
    {
        public DateTime? Convert(Timestamp source, DateTime? destination, ResolutionContext context)
        {
            return source?.ToDateTime();
        }
    }
}
