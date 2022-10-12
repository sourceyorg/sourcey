using Zion.Core.Stores;

namespace Zion.Commands.Stores
{
    public abstract class BufferedCommandStore<TCommandStoreContext> : BufferedStore<ICommand>, ICommandStore<TCommandStoreContext>
    {
    }
}
