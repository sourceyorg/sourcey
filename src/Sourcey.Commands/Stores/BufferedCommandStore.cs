using Sourcey.Core.Stores;

namespace Sourcey.Commands.Stores
{
    public abstract class BufferedCommandStore<TCommandStoreContext> : BufferedStore<ICommand>, ICommandStore<TCommandStoreContext>
    {
    }
}
