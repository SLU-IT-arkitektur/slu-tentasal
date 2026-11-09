using Microsoft.Extensions.Options;
using SluTentasal.Web.Options;

namespace SluTentasal.Web.Data;

/// <summary>Ger "nu" i den tidszon som tentatiderna är lagrade i.</summary>
public sealed class LocalClock(TimeProvider timeProvider, IOptions<TentasalOptions> options)
{
    private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZone);

    public DateTime Now =>
        DateTime.SpecifyKind(
            TimeZoneInfo.ConvertTimeFromUtc(timeProvider.GetUtcNow().UtcDateTime, _timeZone),
            DateTimeKind.Unspecified);
}
