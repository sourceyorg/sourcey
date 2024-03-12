using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sourcey.Initialization;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.ProjectionStates;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Initializers;

internal class ProjectionStateInitializer<TProjection> : ISourceyInitializer
    where TProjection : class, IProjection
{
    public bool ParallelEnabled => false;
    private readonly IProjectionStateDbContextFactory _projectionStateDbContextFactory;
    private readonly ProjectionStateOptions<TProjection> _options;
    private readonly ILogger<ProjectionStateInitializer<TProjection>> _logger;


    public ProjectionStateInitializer(IProjectionStateDbContextFactory projectionStateDbContextFactory,
        ProjectionStateOptions<TProjection> options, 
        ILogger<ProjectionStateInitializer<TProjection>> logger)
    {
        ArgumentNullException.ThrowIfNull(projectionStateDbContextFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _projectionStateDbContextFactory = projectionStateDbContextFactory;
        _options = options;
        _logger = logger;
    }

    public async Task InitializeAsync(IHost host)
    {
        if (!_options.AutoMigrate)
            return;
        
        _logger.LogDebug("Starting - migration {projection}", typeof(TProjection).FullName);
        
        var success = false;
        var attempts = 0;
        
        while (!success && attempts < 10)
        {
            try
            {
                await using var context = _projectionStateDbContextFactory.Create<TProjection>();
                await context.Database.MigrateAsync();
            }
            catch (Exception e)
            {
                _logger.LogDebug("Unable to migrate {projection}, attempts: {attempt}, exception: {exception}", typeof(TProjection).FullName, attempts++, e.Message);
                await Task.Delay(200);
                continue;
            }

            success = true;
        }

        _logger.LogDebug("Finished - migration {projection}", typeof(TProjection).FullName);
    }
}
