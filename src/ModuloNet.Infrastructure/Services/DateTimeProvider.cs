using ModuloNet.Application.Abstractions;

namespace ModuloNet.Infrastructure.Services;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
