namespace SDI.DI.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine.CustomMessages.Tools;
    using Installers;
    using Modules;
    using Resolver;
    using UnityEngine;

    public class SceneDependencyResolver : DependencyResolver, IMessageReceiver
    {
        #region Resolving
        protected override void GetTargetObjects()
        {
            var sceneObjects = DIHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
                typeof(DependencyResolver), 
                typeof(Installer)
            });
            var sceneComponents = DIHelpers.GetAllComponents(sceneObjects);

            foreach (var sceneComponent in sceneComponents)
            {
                var instance = (object)sceneComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        #region Helpers
        private void ResolveWithRefWrapper(object resolvedObject)
        {
            ResolveDependencies(ref resolvedObject);
        }
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
        #region Private Variables
        private StateMachineResolver stateMachineResolver = new StateMachineResolver();
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
    }
    
}