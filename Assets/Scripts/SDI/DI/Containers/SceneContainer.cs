namespace SDI.DI.Containers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine;
    using Core.StateMachine.CustomMessages.Tools;
    using Data;
    using Installers;
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

        #region Public API
        public void InstallDependency(Type targetType, InstallerEntity installerEntity)
        {
            if (registeredDependencies.TryGetValue(targetType, out var entry))
            {
                entry.Add(installerEntity);
                return;
            }
            registeredDependencies.Add(targetType, new List<InstallerEntity> {installerEntity});
        }

        private void ResolveStateMachineDependencies(object stateMachineRunnerInstance)
        {
            var fields = stateMachineRunnerInstance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var genericArgs = stateMachineRunnerInstance.GetType().GetGenericArguments();
            foreach (var propertyInfo in fields)
            {
                if (genericArgs.All(genType => genType != propertyInfo.PropertyType))
                {
                    continue;
                }
                //Debug.Log($"Found generic type: {propertyInfo.PropertyType.GetNiceName()}");
                Type targetType = propertyInfo.PropertyType;
                while (targetType != null || targetType != typeof(StateManager<>))
                {
                    if (targetType.BaseType == null)
                    {
                        break;
                    }

                    targetType = targetType.BaseType;
                    if (!targetType.IsGenericType) continue;
                    //Debug.Log($"{targetType} | {targetType.GetGenericTypeDefinition()} | {targetType.GetGenericTypeDefinition() == typeof(StateManager<>)}");
                    if (targetType.GetGenericTypeDefinition() == typeof(StateManager<>))
                    {
                        break;
                    }
                }
                if (targetType.GetGenericTypeDefinition() != typeof(StateManager<>))
                {
                    continue;
                }
                // this is StateManager<>
                var stateManagerInstance = propertyInfo.GetValue(stateMachineRunnerInstance);
                
                // get states from StateManager instance
                GetStates(stateManagerInstance);
            }
        }
        private void GetStates(object stateManagerInstance)
        {
            var properties = stateManagerInstance.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.PropertyType.IsArray)
                {
                    continue;
                }
                // check if array is the states array
                var elementType = propertyInfo.PropertyType.GetElementType();
                if (elementType == null || !elementType.IsGenericType) continue;
                if (elementType.GetGenericTypeDefinition() == typeof(State<>))
                {
                    //Debug.Log("Found states array!");
                    var statesArray = propertyInfo.GetValue(stateManagerInstance) as object[];
                    ResolveStates(statesArray);
                }
            }
        }

        private void ResolveStates(object[] statesArray)
        {
            //Debug.Log($"States count: {statesArray.Length}");
            foreach (var state in statesArray)
            {
                var stateInstance = state;
                ResolveDependencies(ref stateInstance);
            }
        }

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

        private object TryForceInitializeInstance(Type type)
        {
            return Activator.CreateInstance(type);
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
                var instance = Resolve(fieldInfo.FieldType, injectable.InstanceId);
                //Debug.Log($"Resolving {fieldInfo.GetNiceName()} in {instanceObject.GetType().GetNiceName()}");
                fieldInfo.SetValue(instanceObject, instance);
            }
            ResolveNested(ref instanceObject);
        }

        public object Resolve(Type targetType, string instanceId = "")
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
        #region Private Methods
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
        #region Unused (to be removed)
        private List<GameObject> GetSceneObjects()
        {
            return FindObjectsOfType<GameObject>().ToList();
        }
        private List<MonoBehaviour> GetAllComponents(List<GameObject> sourceObjectsList)
        {
            List<MonoBehaviour> components = new List<MonoBehaviour>();
            foreach (var listItem in sourceObjectsList)
            {
                components.AddRange(listItem.GetComponents<MonoBehaviour>());
            }
            return components;
        }

        private List<object> GetSceneStateMachineRunners(List<MonoBehaviour> sourceList)
        {
            List<object> result = new List<object>();
            foreach (var behaviour in sourceList)
            {
                var fields = behaviour.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var fieldInfo in fields)
                {
                    if (!fieldInfo.FieldType.IsGenericType) continue;
                    if (fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(StateMachineRunner<,>))
                    {
                        var smrInstance = fieldInfo.GetValue(behaviour);
                        result.Add(smrInstance);
                    }
                }
            }

            return result;
        }
        #endregion

        #endregion

        #region Editor
        [BoxGroup("Declared Installers"), Button(ButtonSizes.Large)]
        private void Refresh()
        {
            declaredInstallers = FindObjectsOfType<Installer>().ToList();
        }
        #endregion

        #region Messages
        List<Type> IMessageReceiver.ListenedTypes => new List<Type>(){typeof(StateMachineToolMessages.OnStateRunnerStatusChanged)};

        void IMessageReceiver.OnMessageReceived(object message)
        {
            if (message is StateMachineToolMessages.OnStateRunnerStatusChanged stateRunnerStatusChanged)
            {
                if (stateRunnerStatusChanged.IsInitialized)
                {
                    ResolveStateMachineDependencies(stateRunnerStatusChanged.StateRunner);
                }
            }
        }
        #endregion
    }
    
}