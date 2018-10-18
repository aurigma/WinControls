// Copyright (c) 2018 Aurigma Inc. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

namespace Aurigma.GraphicsMill.WinControls
{
    public enum QueueItemMethodState
    {
        NotStarted,
        Started,
        Finished
    }

    /// <summary>
    /// Determines interface of the IQueue item.
    /// </summary>
    [ResDescription("IQueueItem")]
    public interface IQueueItem
    {
        /// <summary>
        /// Returns item methods count. Each method is supposed to be executed in separate thread asynchronously.
        /// </summary>
        /// <returns>Returns item methods count.</returns>
        [ResDescription("IQueueItem_MethodCount")]
        int MethodCount
        {
            get;
        }

        /// <summary>
        /// Starts execution of the specified method.
        /// </summary>
        /// <param name="methodIndex">Method index.</param>
        [ResDescription("IQueueItem_EvaluateMethod")]
        void EvaluateMethod(int methodIndex);

        /// <summary>
        /// Gets execution status of the specified method.
        /// </summary>
        /// <param name="methodIndex">Method index.</param>
        [ResDescription("IQueueItem_GetMethodState")]
        QueueItemMethodState GetMethodState(int methodIndex);
    }
}