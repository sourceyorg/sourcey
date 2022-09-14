using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Zion.Extensions;

namespace Zion.Testing.Abstractions
{
    public abstract class Specification<TResult> : IAsyncLifetime
    {
        private ExceptionMode _exceptionMode;

        protected readonly ITestOutputHelper _testOutputHelper;

        protected abstract Task<TResult> Given();

        protected abstract Task When();

        protected Exception? Exception { get; private set; }
        protected TResult? Result { get; private set; }

        protected IServiceProvider ServiceProvider { get; private set; }

        protected void RecordExceptions() => _exceptionMode = ExceptionMode.Record;
        protected virtual void BuildServices(IServiceCollection services)
        {
        }

        public Specification(ITestOutputHelper testOutputHelper)
        { 
            _testOutputHelper = testOutputHelper;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            var services = new ServiceCollection();
            services.AddXunitLogging(_testOutputHelper);

            BuildServices(services);

            ServiceProvider = services.BuildServiceProvider();

            await When();

            try
            {
                Result = await Given();
            }
            catch (Exception e)
            {
                if (_exceptionMode == ExceptionMode.Record)
                    Exception = e;
                else
                    throw;
            }
        }
    }

    public abstract class Specification : IAsyncLifetime
    {
        private ExceptionMode _exceptionMode;

        protected readonly ITestOutputHelper _testOutputHelper;

        protected abstract Task Given();

        protected abstract Task When();

        protected Exception? Exception { get; private set; }

        protected IServiceProvider ServiceProvider { get; private set; }
        protected void RecordExceptions() => _exceptionMode = ExceptionMode.Record;

        protected virtual void BuildServices(IServiceCollection services)
        {
        }

        public Specification(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public virtual Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public virtual async Task InitializeAsync()
        {
            var services = new ServiceCollection();
            services.AddXunitLogging(_testOutputHelper);

            BuildServices(services);

            ServiceProvider = services.BuildServiceProvider();

            await When();

            try
            {
                await Given();
            }
            catch (Exception e) when (_exceptionMode == ExceptionMode.Record)
            {
                Exception = e;
            }
        }
    }
}
