#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace Poseidon.StateMachine.PlayerLoopUtils
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
#if UNITY_2019_3_OR_NEWER
    using UnityEngine.LowLevel;
    using PlayerLoopType = UnityEngine.PlayerLoop;
#else
    using UnityEngine.Experimental.LowLevel;
    using PlayerLoopType = UnityEngine.Experimental.PlayerLoop;
#endif

    public static class LoopRunners
    {
        public struct LoopRunnerInitialization { };
        public struct LoopRunnerEarlyUpdate { };
        public struct LoopRunnerFixedUpdate { };
        public struct LoopRunnerPreUpdate { };
        public struct LoopRunnerUpdate { };
        public struct LoopRunnerPreLateUpdate { };
        public struct LoopRunnerPostLateUpdate { };

        // Last

        public struct LoopRunnerLastInitialization { };
        public struct LoopRunnerLastEarlyUpdate { };
        public struct LoopRunnerLastFixedUpdate { };
        public struct LoopRunnerLastPreUpdate { };
        public struct LoopRunnerLastUpdate { };
        public struct LoopRunnerLastPreLateUpdate { };
        public struct LoopRunnerLastPostLateUpdate { };
        
#if UNITY_2020_2_OR_NEWER
        public struct LoopRunnerTimeUpdate { };
        public struct LoopRunnerLastTimeUpdate { };
#endif
    }
    
    public static class PlayerLoopHelper
    {
        static PlayerLoopRunner[] runners;
        
        static PlayerLoopSystem[] InsertRunner(PlayerLoopSystem loopSystem,
            bool injectOnFirst, Type loopRunnerType, PlayerLoopRunner runner)
        {
            var runnerLoop = new PlayerLoopSystem
            {
                type = loopRunnerType,
                updateDelegate = runner.Run
            };

            // Remove items from previous initializations.
            var source = RemoveRunner(loopSystem, loopRunnerType);
            var dest = new PlayerLoopSystem[source.Length + 1];

            Array.Copy(source, 0, dest, injectOnFirst ? 1 : 0, source.Length);
            if (injectOnFirst)
            {
                dest[0] = runnerLoop;
            }
            else
            {
                dest[dest.Length - 1] = runnerLoop;
            }

            return dest;
        }

        private static PlayerLoopSystem[] RemoveRunner(PlayerLoopSystem loopSystem, Type loopRunnerType)
        {
            return loopSystem.subSystemList
                .Where(ls => ls.type != loopRunnerType)
                .ToArray();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {

#if UNITY_EDITOR && UNITY_2019_3_OR_NEWER
            // When domain reload is disabled, re-initialization is required when entering play mode; 
            // otherwise, pending tasks will leak between play mode sessions.
            var domainReloadDisabled = UnityEditor.EditorSettings.enterPlayModeOptionsEnabled &&
                UnityEditor.EditorSettings.enterPlayModeOptions.HasFlag(UnityEditor.EnterPlayModeOptions.DisableDomainReload);
            if (!domainReloadDisabled && runners != null) return;
#else
            if (runners != null) return; // already initialized
#endif

            var playerLoop =
#if UNITY_2019_3_OR_NEWER
                PlayerLoop.GetCurrentPlayerLoop();
#else
                PlayerLoop.GetDefaultPlayerLoop();
#endif

            Initialize(ref playerLoop);
        }


#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        static void InitOnEditor()
        {
            // Execute the play mode init method
            //Init();
            
            PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());

            // register an Editor update delegate, used to forcing playerLoop update
            //EditorApplication.update += ForceEditorPlayerLoopUpdate;
        }

        private static void ForceEditorPlayerLoopUpdate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                // Not in Edit mode, don't interfere
                return;
            }

            // EditorApplication.QueuePlayerLoopUpdate causes performance issue, don't call directly.
            // EditorApplication.QueuePlayerLoopUpdate();

            if (runners != null)
            {
                foreach (var item in runners)
                {
                    if (item != null) item.Run();
                }
            }
        }

#endif

        private static int FindLoopSystemIndex(PlayerLoopSystem[] playerLoopList, Type systemType)
        {
            for (int i = 0; i < playerLoopList.Length; i++)
            {
                if (playerLoopList[i].type == systemType)
                {
                    return i;
                }
            }

            throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
        }

        private static void InsertLoop(PlayerLoopSystem[] copyList, InjectPlayerLoopTimings injectTimings, Type loopType, InjectPlayerLoopTimings targetTimings,
            int index, bool injectOnFirst, Type loopRunnerType, PlayerLoopTiming playerLoopTiming)
        {
            int i = FindLoopSystemIndex(copyList, loopType);
            if ((injectTimings & targetTimings) == targetTimings)
            {
                copyList[i].subSystemList = InsertRunner(copyList[i], injectOnFirst,
                    loopRunnerType, runners[index] = new PlayerLoopRunner(playerLoopTiming));
            }
            else
            {
                copyList[i].subSystemList = RemoveRunner(copyList[i], loopRunnerType);
            }
        }

        private static void Initialize(ref PlayerLoopSystem playerLoop, InjectPlayerLoopTimings injectTimings = InjectPlayerLoopTimings.All)
        {
            Debug.Log("Initialization PlayerLoopHelper");
#if UNITY_2020_2_OR_NEWER
            runners = new PlayerLoopRunner[16];
#else
            runners = new PlayerLoopRunner[14];
#endif

            var copyList = playerLoop.subSystemList.ToArray();

            // Initialization
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Initialization),
                InjectPlayerLoopTimings.Initialization, 0, true, typeof(LoopRunners.LoopRunnerInitialization), PlayerLoopTiming.Initialization);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Initialization),
                InjectPlayerLoopTimings.LastInitialization, 1, false, typeof(LoopRunners.LoopRunnerLastInitialization), PlayerLoopTiming.LastInitialization);

            // EarlyUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.EarlyUpdate),
                InjectPlayerLoopTimings.EarlyUpdate, 2, true, typeof(LoopRunners.LoopRunnerEarlyUpdate), PlayerLoopTiming.EarlyUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.EarlyUpdate),
                InjectPlayerLoopTimings.LastEarlyUpdate, 3, false, typeof(LoopRunners.LoopRunnerLastEarlyUpdate), PlayerLoopTiming.LastEarlyUpdate);

            // FixedUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.FixedUpdate),
                InjectPlayerLoopTimings.FixedUpdate, 4, true, typeof(LoopRunners.LoopRunnerFixedUpdate), PlayerLoopTiming.FixedUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.FixedUpdate),
                InjectPlayerLoopTimings.LastFixedUpdate, 5, false, typeof(LoopRunners.LoopRunnerLastFixedUpdate), PlayerLoopTiming.LastFixedUpdate);

            // PreUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreUpdate),
                InjectPlayerLoopTimings.PreUpdate, 6, true, typeof(LoopRunners.LoopRunnerPreUpdate), PlayerLoopTiming.PreUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreUpdate),
                InjectPlayerLoopTimings.LastPreUpdate, 7, false, typeof(LoopRunners.LoopRunnerLastPreUpdate), PlayerLoopTiming.LastPreUpdate);

            // Update
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Update),
                InjectPlayerLoopTimings.Update, 8, true, typeof(LoopRunners.LoopRunnerUpdate), PlayerLoopTiming.Update);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.Update),
                InjectPlayerLoopTimings.LastUpdate, 9, false, typeof(LoopRunners.LoopRunnerLastUpdate), PlayerLoopTiming.LastUpdate);

            // PreLateUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreLateUpdate),
                InjectPlayerLoopTimings.PreLateUpdate, 10, true, typeof(LoopRunners.LoopRunnerPreLateUpdate), PlayerLoopTiming.PreLateUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PreLateUpdate),
                InjectPlayerLoopTimings.LastPreLateUpdate, 11, false, typeof(LoopRunners.LoopRunnerLastPreLateUpdate), PlayerLoopTiming.LastPreLateUpdate);

            // PostLateUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PostLateUpdate),
                InjectPlayerLoopTimings.PostLateUpdate, 12, true, typeof(LoopRunners.LoopRunnerPostLateUpdate), PlayerLoopTiming.PostLateUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.PostLateUpdate),
                InjectPlayerLoopTimings.LastPostLateUpdate, 13, false, typeof(LoopRunners.LoopRunnerLastPostLateUpdate), PlayerLoopTiming.LastPostLateUpdate);

#if UNITY_2020_2_OR_NEWER
            // TimeUpdate
            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.TimeUpdate),
                InjectPlayerLoopTimings.TimeUpdate, 14, true, typeof(LoopRunners.LoopRunnerTimeUpdate), PlayerLoopTiming.TimeUpdate);

            InsertLoop(copyList, injectTimings, typeof(PlayerLoopType.TimeUpdate),
                InjectPlayerLoopTimings.LastTimeUpdate, 15, false, typeof(LoopRunners.LoopRunnerLastTimeUpdate), PlayerLoopTiming.LastTimeUpdate);
#endif
            playerLoop.subSystemList = copyList;
            PlayerLoop.SetPlayerLoop(playerLoop);
        }

        // Diagnostics helper
#if UNITY_2019_3_OR_NEWER

        public static void DumpCurrentPlayerLoop()
        {
            var playerLoop = UnityEngine.LowLevel.PlayerLoop.GetCurrentPlayerLoop();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"PlayerLoop List");
            foreach (var header in playerLoop.subSystemList)
            {
                sb.AppendFormat("------{0}------", header.type.Name);
                sb.AppendLine();
                foreach (var subSystem in header.subSystemList)
                {
                    sb.AppendFormat("{0}", subSystem.type.Name);
                    sb.AppendLine();

                    if (subSystem.subSystemList != null)
                    {
                        UnityEngine.Debug.LogWarning("More Subsystem:" + subSystem.subSystemList.Length);
                    }
                }
            }

            UnityEngine.Debug.Log(sb.ToString());
        }

#endif

    }
}

