using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Zion.Core.Extensions;
using Zion.Core.Keys;
using Zion.Events;
using Zion.Projections.Serialization;

namespace Zion.Projections.Cache
{
    internal sealed class ProjectionCacheSnapshotWriter : IProjectionCacheSnapshotWriter
    {
        delegate Task WriteDelegate(IServiceScopeFactory scopeFactory, Actor actor, IEnumerable<IEvent> events, CancellationToken cancellationToken);

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IReadOnlyList<WriteDelegate> _writeDelegates;

        public ProjectionCacheSnapshotWriter(IServiceScopeFactory serviceScopeFactory,
            IEnumerable<ProjectionCacheOption> projectionCacheOptions)
        {
            if(serviceScopeFactory is null)
                throw new ArgumentNullException(nameof(serviceScopeFactory));

            var method = typeof(ProjectionCacheSnapshotWriter).GetMethod(nameof(InternalWriteAsync), BindingFlags.Static | BindingFlags.NonPublic);
            _writeDelegates = projectionCacheOptions.Select(o => method.MakeGenericMethod(o.Type).CreateDelegate<WriteDelegate>()).ToImmutableList();
            _scopeFactory = serviceScopeFactory;
        }

        private static async Task InternalWriteAsync<TProjection>(IServiceScopeFactory scopeFactory, Actor actor, IEnumerable<IEvent> events, CancellationToken cancellationToken)
            where TProjection : class, IProjection
        {
            using var scope = scopeFactory.CreateScope();
            var key = typeof(TProjection).FriendlyFullName();
            var projectionManagerFactory = scope.ServiceProvider.GetRequiredService<Func<IServiceProvider, IProjectionWriter<TProjection>[], IProjectionManager<TProjection>>>();

            // Needs to be a projection cache reader
            var projectionReader = scope.ServiceProvider.GetRequiredService<IProjectionReader<TProjection>>();

            var projectionSerializer = scope.ServiceProvider.GetRequiredService<IProjectionSerializer>();
            var distributedCache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
            var projectionWriter = new ProjectionCacheWriter<TProjection>(projectionReader, actor);
            var manager = projectionManagerFactory(scope.ServiceProvider, new IProjectionWriter<TProjection>[] {  });

            foreach (var @event in events)
                await manager.HandleAsync(@event, cancellationToken);

            var caches = projectionWriter.GetProjections().Select(pc => (pc, new ProjectionCacheKey(key, actor, pc.Subject, pc.Events))).ToArray();
            var actorCacheKey = $"{key}:{actor}:caches";

            await distributedCache.RemoveAsync(actorCacheKey, cancellationToken);

            foreach (var projectionCache in caches)
                await distributedCache.SetStringAsync(
                    projectionCache.Item2.ToString(),
                    projectionSerializer.Serialize(projectionCache.pc.Projection),
                    cancellationToken);

            await distributedCache.SetStringAsync(actorCacheKey, JsonSerializer.Serialize(caches.Select(c => c.Item2.ToString())), cancellationToken);
        }

        public async ValueTask WriteAsync(Actor actor, IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
        {
            await Task.WhenAll(_writeDelegates.Select(d => d(_scopeFactory, actor, events, cancellationToken)));
        }
    }


    internal sealed class ProjectionCacheWriter<TProjection> : IProjectionWriter<TProjection>
        where TProjection : class, IProjection
    {
        private readonly IProjectionReader<TProjection> _projectionReader;
        private readonly Actor _actor;
        private readonly List<ProjectionCache<TProjection>> _projections;

        public ProjectionCacheWriter(IProjectionReader<TProjection> projectionReader, Actor actor)
        {
            if (projectionReader is null)
                throw new ArgumentNullException(nameof(projectionReader));

            _projectionReader = projectionReader;
            _actor = actor;
        }

        public Task<TProjection> AddAsync(string subject, Func<TProjection> add, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string subject, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ResetAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<TProjection> UpdateAsync(string subject, Func<TProjection, TProjection> update, CancellationToken cancellationToken = default)
        {
            var projection = await _projectionReader.RetrieveAsync(Subject.From(subject), cancellationToken);
            projection = update(projection);
            return projection;
        }

        public Task<TProjection> UpdateAsync(string subject, Action<TProjection> update, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<ProjectionCache<TProjection>> GetProjections() => _projections;
    }

    internal record ProjectionCache<TProjection>(Subject Subject, TProjection Projection, IOrderedEnumerable<IEvent> Events);
}
