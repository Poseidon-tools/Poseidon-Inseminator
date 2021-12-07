namespace Poseidon.StateMachine.PlayerLoopUtils
{
    using System;

    internal sealed class PlayerLoopRunner
    {
        public static event Action OnInitialization;
        public static event Action OnLastInitialization;
        public static event Action OnEarlyUpdate;
        public static event Action OnLastEarlyUpdate;
        public static event Action OnFixedUpdate;
        public static event Action OnLastFixedUpdate;
        public static event Action OnPreUpdate;
        public static event Action OnLastPreUpdate;
        public static event Action OnUpdate;
        public static event Action OnLastUpdate;
        public static event Action OnPreLateUpdate;
        public static event Action OnLastPreLateUpdate;
        public static event Action OnPostLateUpdate;
        public static event Action OnLastPostLateUpdate;
#if UNITY_2020_2_OR_NEWER
        public static event Action OnTimeUpdate;
        public static event Action OnLastTimeUpdate;
#endif

        readonly PlayerLoopTiming timing;

        public PlayerLoopRunner(PlayerLoopTiming timing)
        {
            this.timing = timing;
        }
        
        public void Run()
        {
            switch (timing)
            {
                case PlayerLoopTiming.Initialization: OnInitialization?.Invoke(); 
                    break;
                case PlayerLoopTiming.LastInitialization: OnLastInitialization?.Invoke(); 
                    break;
                case PlayerLoopTiming.EarlyUpdate: OnEarlyUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastEarlyUpdate: OnLastEarlyUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.FixedUpdate: OnFixedUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastFixedUpdate: OnLastFixedUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.PreUpdate: OnPreUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastPreUpdate: OnLastPreUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.Update: OnUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastUpdate: OnLastUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.PreLateUpdate: OnPreLateUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastPreLateUpdate: OnLastPreLateUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.PostLateUpdate: OnPostLateUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastPostLateUpdate: OnLastPostLateUpdate?.Invoke();
                    break;
#if UNITY_2020_2_OR_NEWER
                case PlayerLoopTiming.TimeUpdate: OnTimeUpdate?.Invoke();
                    break;
                case PlayerLoopTiming.LastTimeUpdate: OnLastTimeUpdate?.Invoke();
                    break;
#endif
                default:
                    break;
            }
        }
    }
}
