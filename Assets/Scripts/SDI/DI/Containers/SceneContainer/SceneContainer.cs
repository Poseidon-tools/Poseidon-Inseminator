namespace SDI.DI.Containers.SceneContainer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine.CustomMessages.Tools;
    using Data;
    using Installers;
    using Modules;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Utils;

    public class SceneContainer : SerializedMonoBehaviour, IMessageReceiver
    {
        #region Inspector
        [SerializeField, BoxGroup("Declared Installers"), InfoBox("Remember to drag your installer to this list!", InfoMessageType.Warning)]
        [HideLabel]
        private List<Installer> declaredInstallers = new List<Installer>();
        #endregion
        #region Private Variables
        private Dictionary<Type, List<InstallerEntity>> registeredDependencies = new Dictionary<Type, List<InstallerEntity>>();
        #endregion

        #region Installing API
        private void InstallDependency(Type targetType, InstallerEntity installerEntity)
        {
            if (registeredDependencies.TryGetValue(targetType, out var entry))
            {
                entry.Add(installerEntity);
                return;
            }
            registeredDependencies.Add(targetType, new List<InstallerEntity> {installerEntity});
        }

        private void Install(List<Installer> installers)
        {
            foreach (var installer in installers)
            {
                installer.CreateBindings();
                foreach (var installerBinding in installer.InstallerBindings)
                {
                    foreach (var installerEntity in installerBinding.Value)
                    {
                        InstallDependency(installerBinding.Key, installerEntity);
                    }
                }
            }
        }
        #endregion

        #region Resolving
        private void ResolveNested(ref object parentInstance)
        {
            var fields = parentInstance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            //Debug.Log($"{parentInstance.GetType().GetNiceName()} nested:");
            foreach (var fieldInfo in fields)
            {
                var nestedInjectAttribute = fieldInfo.GetCustomAttribute<Attributes.NestedInjectable>();
                if (nestedInjectAttribute == null)
                {
                    continue;
                }
                
                var nestedInstance = fieldInfo.GetValue(parentInstance);
                if (nestedInjectAttribute.ForceInitialization)
                {
                    nestedInstance = TryForceInitializeInstance(fieldInfo.FieldType);
                    if(nestedInstance == null)
                    {
                        Debug.Log("Cannot create instance.");
                        continue;
                    }
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
                    
                //Debug.Log($"Resolving nested dependencies in {fieldInfo.FieldType.GetNiceName()}");
                ResolveDependencies(ref nestedInstance);
                if (fieldInfo.FieldType.IsValueType)
                {
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
            }
        }
        protected void ResolveDependencies(ref object instanceObject)
        {
            var allInjectableFields = instanceObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            //Debug.Log($"{instanceObject.GetType().GetNiceName()} fields count: {allInjectableFields.Length}");
            foreach (var fieldInfo in allInjectableFields)
            {
                var injectAttribute = fieldInfo.GetCustomAttribute(typeof(Attributes.Injectable), true);
                if (injectAttribute == null)
                {
                    continue;
                }

                if (!(injectAttribute is Attributes.Injectable injectable)) continue;
                var instance = ResolveSingleDependency(fieldInfo.FieldType, injectable.InstanceId);
                //Debug.Log($"Resolving {fieldInfo.GetNiceName()} in {instanceObject.GetType().GetNiceName()}");
                fieldInfo.SetValue(instanceObject, instance);
            }
            ResolveNested(ref instanceObject);
        }

        private object ResolveSingleDependency(Type targetType, string instanceId = "")
        {
            if (!registeredDependencies.TryGetValue(targetType, out var dependency)) return default;
            if (instanceId.IsNullOrEmpty())
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }
        #endregion
        #region Helpers
        private void ResolveWithRefWrapper(object resolvedObject)
        {
            ResolveDependencies(ref resolvedObject);
        }
        
        private object TryForceInitializeInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
        #endregion
        
        #region Unity Methods
        private void Awake()
        {
            MessageDispatcher.Instance.RegisterReceiver(this);
            Install(declaredInstallers);
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