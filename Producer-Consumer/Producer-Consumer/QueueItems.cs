﻿namespace Producer_Consumer
{
    internal static class QueueItems
    {
        internal class Example : IQueueItem
        {
            public void Do()
            {
                // Heavy process
            }
        }
    }
}