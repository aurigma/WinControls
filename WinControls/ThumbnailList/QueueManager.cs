// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//
using System;
using System.Collections;

namespace Aurigma.GraphicsMill.WinControls
{
    /// <summary>
    /// Represents class that manages number of queues. It unites work with all queues into a single object.
    /// </summary>
    [ResDescription("QueueManager")]
    public class QueueManager
    {
        /// <summary>
        /// Initializes new instance of the QueueManager class.
        /// </summary>
        internal QueueManager()
        {
        }

        /// <summary>
        /// Moves specified item to the top in specified queue.
        /// </summary>
        /// <param name="item">Item to move.</param>
        /// <param name="methodIndex">Queue index.</param>
        [ResDescription("QueueManager_MoveToHead")]
        public void MoveToHead(IQueueItem item, int methodIndex)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            UpdateQueuesArray(item.MethodCount);
            ((Queue)_queues[methodIndex]).MoveToHead(item);
        }

        /// <summary>
        /// Starts execution of all queues.
        /// </summary>
        [ResDescription("QueueManager_StartQueues")]
        public void StartQueues()
        {
            for (int i = 0; i < _queues.Count; i++)
                ((Queue)_queues[i]).StartQueue();
        }

        /// <summary>
        /// Adds new item into all queues of the manager.
        /// </summary>
        /// <param name="item">New queue item.</param>
        internal void Add(IQueueItem item)
        {
            if (item == null)
                return;

            UpdateQueuesArray(item.MethodCount);
            for (int i = 0; i < _queues.Count; i++)
            {
                ((Queue)_queues[i]).Add(item);
            }
        }

        /// <summary>
        /// Removes specified item from all queues of the manager.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        internal void Remove(IQueueItem item)
        {
            if (item == null)
                return;

            UpdateQueuesArray(item.MethodCount);
            for (int i = 0; i < _queues.Count; i++)
            {
                ((Queue)_queues[i]).Remove(item);
            }
        }

        /// <summary>
        /// Removes all queues from the manager.
        /// </summary>
        internal void Clear()
        {
            for (int i = 0; i < _queues.Count; i++)
                ((Queue)_queues[i]).Clear();
        }

        #region Private methods

        /// <summary>
        /// Checks count of queues that already have been created in the manager. Creates new ones if necessary.
        /// </summary>
        /// <param name="queueCount">Number of queues that should be presented in manager's queues pool.</param>
        private void UpdateQueuesArray(int queueCount)
        {
            if (_queues.Count >= queueCount)
                return;

            for (int i = _queues.Count; i < queueCount; i++)
            {
                _queues.Add(new Queue(i));
            }
        }

        #endregion Private methods

        #region Private members

        /// <summary>
        /// Queues array.
        /// </summary>
        private ArrayList _queues = new ArrayList(3);

        #endregion Private members
    }
}