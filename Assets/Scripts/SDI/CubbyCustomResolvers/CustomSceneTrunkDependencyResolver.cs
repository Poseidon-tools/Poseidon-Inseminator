namespace SDI.CubbyCustomResolvers
{
    using System;
    using System.Collections.Generic;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine.CustomMessages.Tools;
    using Modules;
    using trunkDI.Scripts.DependencyResolvers.Scene;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class CustomSceneTrunkDependencyResolver : SceneTrunkDependencyResolver, IMessageReceiver
    {
        #region Private Variables
        private StateMachineResolver stateMachineResolver = new StateMachineResolver();
        #endregion
        #region Unity Methods
        private void Awake()
        {
            MessageDispatcher.Instance.RegisterReceiver(this);
        }

        private void OnDisable()
        {
            MessageDispatcher.Instance.UnregisterReceiver(this);
        }
        #endregion
        #region Runtime Messages
        List<Type> IMessageReceiver.ListenedTypes => new List<Type>(){typeof(StateMachineToolMessages.OnStateRunnerStatusChanged)};

        void IMessageReceiver.OnMessageReceived(object message)
        {
            if (message is StateMachineToolMessages.OnStateRunnerStatusChanged stateRunnerStatusChanged)
            {
                if (stateRunnerStatusChanged.IsInitialized)
                {
                    stateMachineResolver.ResolveStateMachineDependencies(stateRunnerStatusChanged.StateRunner, ResolveWithRefWrapper);
                }
            }
        }
        #endregion
        #region Helpers
        private void ResolveWithRefWrapper(object resolvedObject)
        {
            ResolveDependencies(ref resolvedObject);
        }
        #endregion
    }
}