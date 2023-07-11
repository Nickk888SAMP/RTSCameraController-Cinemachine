using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using Unity.Services.Core.Internal;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace Unity.Services.Core.Scheduler.Internal
{
    class ScheduledInvocation : IComparable<ScheduledInvocation>
    {
        public Action Action;
        public DateTime InvocationTime;
        public long ActionId;

        public int CompareTo(ScheduledInvocation that)
        {
            var compareResult = InvocationTime.CompareTo(that.InvocationTime);

            //Actions with same invocation time will execute in id order (schedule order).
            if (compareResult == 0)
            {
                compareResult = ActionId.CompareTo(that.ActionId);
            }

            return compareResult;
        }
    }

    class ActionScheduler : IActionScheduler
    {
        readonly ITimeProvider m_TimeProvider;

        readonly MinimumBinaryHeap<ScheduledInvocation> m_ScheduledActions
            = new MinimumBinaryHeap<ScheduledInvocation>();

        readonly Dictionary<long, ScheduledInvocation> m_IdScheduledInvocationMap
            = new Dictionary<long, ScheduledInvocation>();

        const long k_MinimumIdValue = 1;

        internal readonly PlayerLoopSystem SchedulerLoopSystem;

        long m_NextId = k_MinimumIdValue;

        public ActionScheduler()
            : this(new UtcTimeProvider()) {}

        public ActionScheduler(ITimeProvider timeProvider)
        {
            m_TimeProvider = timeProvider;
            SchedulerLoopSystem = new PlayerLoopSystem
            {
                type = typeof(ActionScheduler),
                updateDelegate = ExecuteExpiredActions
            };
        }

        public int ScheduledActionsCount => m_ScheduledActions.Count;

        public long ScheduleAction([NotNull] Action action, double delaySeconds = 0)
        {
            if (delaySeconds < 0)
            {
                throw new ArgumentException("delaySeconds can not be negative");
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var scheduledInvocation = new ScheduledInvocation
            {
                Action = action,
                InvocationTime = m_TimeProvider.Now.AddSeconds(delaySeconds),
                ActionId = m_NextId++
            };

            if (m_NextId < k_MinimumIdValue)
            {
                m_NextId = k_MinimumIdValue;
            }

            m_ScheduledActions.Insert(scheduledInvocation);
            m_IdScheduledInvocationMap.Add(scheduledInvocation.ActionId, scheduledInvocation);

            return scheduledInvocation.ActionId;
        }

        public void CancelAction(long actionId)
        {
            if (!m_IdScheduledInvocationMap.ContainsKey(actionId))
            {
                return;
            }

            var scheduledInvocation = m_IdScheduledInvocationMap[actionId];
            m_ScheduledActions.Remove(scheduledInvocation);
            m_IdScheduledInvocationMap.Remove(scheduledInvocation.ActionId);
        }

        internal void ExecuteExpiredActions()
        {
            while (m_ScheduledActions.Count > 0
                   && m_ScheduledActions.Min.InvocationTime <= m_TimeProvider.Now)
            {
                var scheduledInvocation = m_ScheduledActions.ExtractMin();
                m_IdScheduledInvocationMap.Remove(scheduledInvocation.ActionId);
                try
                {
                    scheduledInvocation.Action();
                }
                catch (Exception e)
                {
                    CoreLogger.LogException(e);
                }
            }
        }

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ClearActionSchedulerFromPlayerLoop()
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var currentSubSystems = new List<PlayerLoopSystem>(currentPlayerLoop.subSystemList);
            for (var i = currentSubSystems.Count - 1; i >= 0; i--)
            {
                if (currentSubSystems[i].type == typeof(ActionScheduler))
                {
                    currentSubSystems.RemoveAt(i);
                }
            }

            UpdateSubSystemList(currentSubSystems, currentPlayerLoop);
        }

#endif

        static void UpdateSubSystemList(List<PlayerLoopSystem> subSystemList, PlayerLoopSystem currentPlayerLoop)
        {
            currentPlayerLoop.subSystemList = subSystemList.ToArray();
            PlayerLoop.SetPlayerLoop(currentPlayerLoop);
        }

        public void JoinPlayerLoopSystem()
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var currentSubSystems = new List<PlayerLoopSystem>(currentPlayerLoop.subSystemList);
            if (!currentSubSystems.Contains(SchedulerLoopSystem))
            {
                currentSubSystems.Add(SchedulerLoopSystem);
                UpdateSubSystemList(currentSubSystems, currentPlayerLoop);
            }
        }

        public void QuitPlayerLoopSystem()
        {
            var currentPlayerLoop = PlayerLoop.GetCurrentPlayerLoop();
            var currentSubSystems = new List<PlayerLoopSystem>(currentPlayerLoop.subSystemList);
            currentSubSystems.Remove(SchedulerLoopSystem);
            UpdateSubSystemList(currentSubSystems, currentPlayerLoop);
        }
    }
}
