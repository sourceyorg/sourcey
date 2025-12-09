using Microsoft.Extensions.DependencyInjection;
using Sourcey.Events;
using Sourcey.Events.Cache;
using Sourcey.Keys;

namespace Sourcey.Tests.Events.Cache
{
    public class WhenResolvingEventTypes
    {
        private static ServiceProvider BuildProvider(params Type[] eventTypes)
        {
            var services = new ServiceCollection();
            services.AddSourcey(b =>
            {
                b.AddEvents(e => e.RegisterEventCache(eventTypes));
            });

            return services.BuildServiceProvider();
        }

        [Then]
        public void Then_simple_name_exists_case_insensitively_TryGet_returns_type()
        {
            using var provider = BuildProvider(typeof(UniqueEvent));
            var cache = provider.GetRequiredService<IEventTypeCache>();

            cache.TryGet("uniqueevent", out var type).ShouldBeTrue();
            type.ShouldBe(typeof(UniqueEvent));
        }

        [Then]
        public void Then_name_is_fully_qualified_TryGet_returns_false()
        {
            using var provider = BuildProvider(typeof(UniqueEvent));
            var cache = provider.GetRequiredService<IEventTypeCache>();

            cache.TryGet(typeof(UniqueEvent).FullName!, out var type).ShouldBeFalse();
            type.ShouldBeNull();
        }

        [Then]
        public void Then_simple_name_is_ambiguous_TryGet_returns_first_registered_type()
        {
            using var provider = BuildProvider(
                typeof(Sourcey.Tests.Events.Cache.A.NamespaceCollision),
                typeof(Sourcey.Tests.Events.Cache.B.NamespaceCollision));
            var cache = provider.GetRequiredService<IEventTypeCache>();

            cache.TryGet("NamespaceCollision", out var type).ShouldBeTrue();
            type.ShouldBe(typeof(Sourcey.Tests.Events.Cache.A.NamespaceCollision));
        }

        [Then]
        public void Then_simple_name_is_ambiguous_ContainsKey_returns_true()
        {
            using var provider = BuildProvider(
                typeof(Sourcey.Tests.Events.Cache.A.NamespaceCollision),
                typeof(Sourcey.Tests.Events.Cache.B.NamespaceCollision));
            var cache = provider.GetRequiredService<IEventTypeCache>();

            cache.ContainsKey("NamespaceCollision").ShouldBeTrue();
        }

        [Then]
        public void Then_name_not_found_TryGet_returns_false_and_type_null()
        {
            using var provider = BuildProvider(typeof(UniqueEvent));
            var cache = provider.GetRequiredService<IEventTypeCache>();

            cache.TryGet("DoesNotExist", out var type).ShouldBeFalse();
            type.ShouldBeNull();
        }

        // Dummy events for registration only; instances are not needed for cache behavior
        public sealed class UniqueEvent : IEvent
        {
            public EventId Id { get; } = default;
            public StreamId StreamId { get; } = default;
            public DateTimeOffset Timestamp { get; } = default;
            public int? Version { get; } = default;
        }
    }
}

namespace Sourcey.Tests.Events.Cache.A
{
    using Sourcey.Events;
    using Sourcey.Keys;

    public sealed class NamespaceCollision : IEvent
    {
        public EventId Id { get; } = default;
        public StreamId StreamId { get; } = default;
        public DateTimeOffset Timestamp { get; } = default;
        public int? Version { get; } = default;
    }
}

namespace Sourcey.Tests.Events.Cache.B
{
    using Sourcey.Events;
    using Sourcey.Keys;

    public sealed class NamespaceCollision : IEvent
    {
        public EventId Id { get; } = default;
        public StreamId StreamId { get; } = default;
        public DateTimeOffset Timestamp { get; } = default;
        public int? Version { get; } = default;
    }
}
