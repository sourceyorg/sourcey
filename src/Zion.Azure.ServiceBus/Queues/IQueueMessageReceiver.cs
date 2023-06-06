﻿using Microsoft.Azure.ServiceBus;

namespace Zion.Azure.ServiceBus.Queues
{
    public interface IQueueMessageReceiver
    {
        Task ReceiveAsync(IQueueClient client, Message message, CancellationToken cancellationToken = default);
        Task OnErrorAsync(ExceptionReceivedEventArgs error);
    }
}