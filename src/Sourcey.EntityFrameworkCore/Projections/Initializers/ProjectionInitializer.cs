using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sourcey.EntityFrameworkCore.Projections.Factories.DbContexts.Writeable;
using Sourcey.Initialization;
using Sourcey.Projections;

namespace Sourcey.EntityFrameworkCore.Projections.Initializers;

internal class ProjectionInitializer<TProjection> : ISourceyInitializer
    where TProjection : class, IProjection
{
    public bool ParallelEnabled => false;
    private readonly IWriteableProjectionDbContextFactory _projectionDbContextFactory;
    private readonly ProjectionOptions<TProjection> _options;
    private readonly ILogger<ProjectionInitializer<TProjection>> _logger;


    public ProjectionInitializer(IWriteableProjectionDbContextFactory projectionDbContextFactory,
        ProjectionOptions<TProjection> options,
        ILogger<ProjectionInitializer<TProjection>> logger)
    {
        ArgumentNullException.ThrowIfNull(projectionDbContextFactory);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(logger);

        _projectionDbContextFactory = projectionDbContextFactory;
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
                var context = _projectionDbContextFactory.Create<TProjection>();
                await using (context.ConfigureAwait(false))
                {
                    await context.Database.MigrateAsync().ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                _logger.LogDebug("Unable to migrate {projection}, attempts: {attempt}, exception: {exception}", typeof(TProjection).FullName, attempts++, e.Message);
                await Task.Delay(200).ConfigureAwait(false);
                continue;
            }

            success = true;
        }

        _logger.LogDebug("Finished - migration {projection}", typeof(TProjection).FullName);
    }
}
