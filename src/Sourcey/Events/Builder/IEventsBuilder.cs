using Microsoft.Extensions.DependencyInjection;

namespace Sourcey.Events.Builder;

public interface IEventsBuilder
{
    IServiceCollection Services { get; }
    IEventsBuilder RegisterEventCache<TEvent>() where TEvent : IEvent;
    IEventsBuilder RegisterEventCache(params Type[] types);
    IEventsBuilder RegisterEventCache();
}