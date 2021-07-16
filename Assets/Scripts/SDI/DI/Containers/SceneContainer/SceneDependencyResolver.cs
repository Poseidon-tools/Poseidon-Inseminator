namespace SDI.DI.Containers.SceneContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine.CustomMessages.Tools;
    using Installers;
    using Modules;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class SceneDependencyResolver : DependencyResolver, IMessageReceiver
    {
        #region Inspector
        [SerializeField, BoxGroup("Declared Installers"), InfoBox("Remember to drag your installer to this list!", InfoMessageType.Warning)]
        [HideLabel]
        private List<Installer> declaredInstallers = new List<Installer>();
        #endregion

        #region Resolving
        private void ResolveSceneObjects()
        {
            var sceneObjects = DIHelpers.GetSceneObjects();
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
            Install(declaredInstallers);

            ResolveSceneObjects();
        }
        private void OnDisable()
        {
            MessageDispatcher.Instance.UnregisterReceiver(this);
        }
        #endregion
        #region Private Variables
        private StateMachineResolver stateMachineResolver = new StateMachineResolver();
        #endregion

        #region Editor
        [BoxGroup("Declared Installers"), Button(ButtonSizes.Large)]
        private void Refresh()
        {
            declaredInstallers = FindObjectsOfType<Installer>().ToList();
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
    }
    
}