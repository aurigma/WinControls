// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Queue class for handling asynchronous tasks processing. Each item of the queue represents
    /// number of associated tasks. Each class instance processes only one task of each item (its number should be provided
    /// in constructor). Items are processed in FIFO order, but it can be changed on demand (using MoveToHead
    /// method to execute item's task as soon as possible).
    /// </summary>
    internal class Queue
    {
        /// <summary>
        /// Initializes new instance of the Queue class.
        /// </summary>
        /// <param name="_methodIndex">Determines index of task that will be executed by the queue object.</param>
        internal Queue(int methodIndex)
        {
            _methodIndex = methodIndex;
        }

        /// <summary>
        /// Adds new item to the queue.
        /// </summary>
        /// <param name="item">The item to add.</param>
        internal void Add(IQueueItem item)
        {
            lock (this)
            {
                _queueItems.Add(item);
            }
        }

        /// <summary>
        /// Removes specified item from the queue.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        internal void Remove(IQueueItem item)
        {
            lock (this)
            {
                while (_queueItems.Contains(item))
                {
                    _queueItems.Remove(item);
                }
            }
        }

        /// <summary>
        /// Moves specified item to the top of the queue, so it will be processes as soon as possible.
        /// </summary>
        /// <param name="item">The Item to move.</param>
        internal void MoveToHead(IQueueItem item)
        {
            if (item == null)
                return;

            lock (this)
            {
                _queueItems.Remove(item);
                _queueItems.Insert(0, item);
            }
        }

        /// <summary>
        /// Returns top element of the queue and also removes it from the queue.
        /// </summary>
        /// <returns>Top element of the queue.</returns>
        internal IQueueItem PopHeadElement()
        {
            IQueueItem itemResult = null;
            lock (this)
            {
                if (IsEmpty())
                    return null;

                itemResult = _queueItems[0] as IQueueItem;
                if (itemResult != null)
                    _queueItems.RemoveAt(0);
            }
            return itemResult;
        }

        /// <summary>
        /// Returns true of queue is empty; otherwise, false.
        /// </summary>
        /// <returns>Returns true of queue is empty; otherwise, false.</returns>
        internal bool IsEmpty()
        {
            bool result = false;
            lock (this)
            {
                result = (_queueItems.Count == 0);
            }
            return result;
        }

        /// <summary>
        /// Removes all elements from the queue.
        /// </summary>
        internal void Clear()
        {
            lock (_startSyncronizer)
            {
                Stop();
                if (_queueThread != null)
                {
                    _queueThread.Join();
                    if (_queueThread.IsAlive)
                        throw new Aurigma.GraphicsMill.UnexpectedException("Thread shouldn't be alive.");
                }
                _queueThread = null;

                lock (this)
                {
                    _queueItems.Clear();
                }
            }
        }

        internal void Stop()
        {
            _stop = true;
        }

        /// <summary>
        /// Starts queue execution. If execution already have been started, nothing happens.
        /// </summary>
        internal void StartQueue()
        {
            lock (_startSyncronizer)
            {
                lock (this)
                {
                    if (_isRunning)
                        return;

                    _isRunning = true;

                    _stop = false;

                    System.Threading.ThreadStart start = new System.Threading.ThreadStart(EvaluateQueue);
                    _queueThread = new System.Threading.Thread(start);
                    _queueThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    _queueThread.Priority = System.Threading.ThreadPriority.Normal;
                    _queueThread.Start();
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("ole32.dll")]
        private static extern int CoInitialize(IntPtr pvReserved);

        /// <summary>
        /// Main queue processing method. Processes all items in FIFO order.
        /// </summary>
        private void EvaluateQueue()
        {
            while (true)
            {
                IQueueItem item;
                lock (this)
                {
                    item = PopHeadElement();
                    if (item == null)
                    {
                        _isRunning = false;
                        break;
                    }
                }

                if (item.GetMethodState(_methodIndex) == QueueItemMethodState.Started)
                    continue;
                else
                {
                    item.EvaluateMethod(_methodIndex);
                }

                lock (this)
                {
                    if (_stop)
                    {
                        _stop = false;
                        _isRunning = false;
                        break;
                    }
                }
            }
        }

        #region "Class variables"

        /// <summary>
        /// Index of method to execute.
        /// </summary>
        private int _methodIndex = -1;

        /// <summary>
        /// Queue items.
        /// </summary>
        private ArrayList _queueItems = new ArrayList();

        /// <summary>
        /// Thread of the queue
        /// </summary>
        private System.Threading.Thread _queueThread;

        private object _startSyncronizer = new object();

        private bool _isRunning = false;

        private bool _stop;

        #endregion "Class variables"
    }
}