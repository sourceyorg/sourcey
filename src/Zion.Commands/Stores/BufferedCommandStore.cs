using Zion.Core.Stores;

namespace Zion.Commands.Stores
{
    public abstract class BufferedCommandStore : BufferedStore<ICommand>, ICommandStore
    {
    }
}
