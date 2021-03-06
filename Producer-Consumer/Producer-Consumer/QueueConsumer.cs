﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Producer_Consumer
{
    internal class QueueConsumer : IDisposable
    {
        private BlockingCollection<IQueueItem> _queue;
        private CancellationTokenSource _processCancellationTokenSource;
        private ManualResetEvent _queueAddedEvent;

        internal bool IsRunning { get; private set; }

        private void Process()
        {
            while (!(_processCancellationTokenSource?.IsCancellationRequested ?? true))
            {
                if (_queue?.Count > 0)
                {
                    do
                    {
                        IQueueItem item = _queue?.Take();
                        item?.Do();

                        if (_processCancellationTokenSource?.IsCancellationRequested ?? true)
                        {
                            break;
                        }
                    } while (_queue?.Count > 0);
                    
                    _queueAddedEvent?.Reset();
                    continue;
                }

                if (_processCancellationTokenSource != null && _queueAddedEvent != null)
                {
                    Debug.WriteLine("Waiting for add next item...");
                    int ret = WaitHandle.WaitAny(new[] {_processCancellationTokenSource.Token.WaitHandle, _queueAddedEvent});
                    if (ret == 0)
                    {
                        break;
                    }

                    if (_queue?.Count == 0)
                    {
                        _queueAddedEvent?.Reset();
                    }
                }
                else
                {
                    break;
                }
                Debug.WriteLine("Queue item arrived");
            }
            Debug.WriteLine("Process task is finished");
        }

        internal void Add(IQueueItem item)
        {
            _queue.Add(item, _processCancellationTokenSource.Token);
            _queueAddedEvent?.Set();
        }

        internal void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                if (_processCancellationTokenSource != null)
                {
                    _processCancellationTokenSource.Dispose();
                    _processCancellationTokenSource = null;
                }
                if (_processCancellationTokenSource == null)
                {
                    _processCancellationTokenSource = new CancellationTokenSource();
                }
                
                Task.Run((Action)Process, _processCancellationTokenSource.Token);
            }
        }
        internal void Stop()
        {
            if (IsRunning)
            {
                if (!(_processCancellationTokenSource?.IsCancellationRequested ?? true))
                {
                    _processCancellationTokenSource?.CancelAfter(0);
                }
                IsRunning = false;
            }
        }

        internal QueueConsumer()
        {
            _queue = new BlockingCollection<IQueueItem>();
            _queueAddedEvent = new ManualResetEvent(false);
        }

        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Stop();
                    if (_queue != null)
                    {
                        _queue.Dispose();
                    }
                    if (_processCancellationTokenSource != null)
                    {
                        _processCancellationTokenSource.Dispose();
                    }
                    if (_queueAddedEvent != null)
                    {
                        _queueAddedEvent.Dispose();
                    }
                }
                _queue = null;
                _processCancellationTokenSource = null;
                _queueAddedEvent = null;
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}