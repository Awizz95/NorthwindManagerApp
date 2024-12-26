using AutoMapper;
using Google.Protobuf.WellKnownTypes;

namespace Northwind.API.Infrastructure.Mapper
{
    public class DateTimeToTimestampConverter : ITypeConverter<DateTime?, Timestamp>
    {
        public Timestamp Convert(DateTime? source, Timestamp destination, ResolutionContext context)
        {
            if (source.HasValue)
            {
                var utcDateTime = source.Value.ToUniversalTime();

                return Timestamp.FromDateTime(utcDateTime);
            }

            return null;
        }
    }
}
