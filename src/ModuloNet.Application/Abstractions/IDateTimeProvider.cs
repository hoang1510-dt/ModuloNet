namespace ModuloNet.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
