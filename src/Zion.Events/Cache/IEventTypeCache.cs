﻿namespace Zion.Events.Cache
{
    public interface IEventTypeCache
    {
        bool TryGet(string name, out Type? type);
    }
}
