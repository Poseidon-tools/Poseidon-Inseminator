namespace CubbyDI.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine.CustomMessages.Tools;
    using Data;
    using Installers;
    using Modules;
    using Resolver;
    using UnityEngine;

    //todo: extract custom StateManager resolving logic, make derived class
    public class SceneDependencyResolver : DependencyResolver, IMessageReceiver
    {
        #region Public API
        public void ResolveExternalGameObject(ref GameObject externalInstance)
        {
            var components = CubbyHelpers.GetAllComponents(new List<GameObject>() {externalInstance});
            foreach (var externalComponent in components)
            {
                var instance = (object)externalComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        #region Resolving
        protected override void GetTargetObjects()
        {
            var sceneObjects = CubbyHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
                typeof(DependencyResolver), 
                typeof(CubbyInstaller)
            }, gameObject.scene);
            var sceneComponents = CubbyHelpers.GetAllComponents(sceneObjects);

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
            //add self to dependencies
            registeredDependencies.Add(GetType(), new List<InstallerEntity>
            {
                new InstallerEntity
                {
                    Id = "",
                    ObjectInstance = this
                }
            });
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