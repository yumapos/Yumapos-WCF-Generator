using System;

namespace TestRepositoryGeneration.Infrastructure
{
    public interface IDateTimeService : IDateTimeProvider
    {
    }

    public interface IDateTimeProvider
    {
        System.DateTime CurrentDateTime { get; }

        System.DateTime CurrentDateTimeUtc { get; }

        DateTimeOffset CurrentDateTimeOffset { get; }
        DateTimeOffset CurrentDateTimeUtcOffset { get; }

        System.DateTime ToUniversalTime(System.DateTime toConvert);

    }
}