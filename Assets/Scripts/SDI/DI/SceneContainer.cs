namespace SDI.DI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.MessageDispatcher;
    using Core.MessageDispatcher.Interfaces;
    using Core.StateMachine;
    using Core.StateMachine.CustomMessages.Tools;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;
    using UnityEngine;
    using Utils;

    public class SceneContainer : SerializedMonoBehaviour, IMessageReceiver
    {
        #region Inspector
        [SerializeField] private Dictionary<Type, List<InstallerEntity>> registeredDependencies = new Dictionary<Type, List<InstallerEntity>>();
        private List<Type> listenedTypes;
        #endregion

        #region Public API
        public void RegisterDependency(Type targetType, InstallerEntity installerEntity)
        {
            if (registeredDependencies.TryGetValue(targetType, out var entry))
            {
                entry.Add(installerEntity);
                return;
            }
            registeredDependencies.Add(targetType, new List<InstallerEntity> {installerEntity});
        }
        
        //todo: unregister?

        public void ResolveStateMachineDependencies(object stateMachineRunnerInstance)
        {
            //Debug.Log($"Trying to resolve...");
            var fields = stateMachineRunnerInstance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            var genericArgs = stateMachineRunnerInstance.GetType().GetGenericArguments();
            foreach (var propertyInfo in fields)
            {
                //Debug.Log($"{propertyInfo.GetNiceName()}:");
                if (genericArgs.All(genType => genType != propertyInfo.PropertyType))
                {
                    //Debug.Log($"Not generic!");
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
                    //Debug.Log("still nothing");
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

                /*
                 * Rectangle rectangle = new Rectangle();
                    PropertyInfo propertyInfo = typeof(Rectangle).GetProperty("Height");
                    object boxed = rectangle;
                    propertyInfo.SetValue(boxed, 5, null);
                    rectangle = (Rectangle) boxed;
                 */
                    
                
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
                    

                Debug.Log($"Resolving nested dependencies in {fieldInfo.FieldType.GetNiceName()}");
                
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
            Debug.Log($"{instanceObject.GetType().GetNiceName()} fields count: {allInjectableFields.Length}");
            foreach (var fieldInfo in allInjectableFields)
            {
                var injectAttribute = fieldInfo.GetCustomAttribute(typeof(Attributes.Injectable), true);
                if (injectAttribute == null)
                {
                    continue;
                }

                if (!(injectAttribute is Attributes.Injectable injectable)) continue;
                var instance = Resolve(fieldInfo.FieldType, injectable.InstanceId);
                Debug.Log($"Resolving {fieldInfo.GetNiceName()} in {instanceObject.GetType().GetNiceName()}");
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
            //Install();
            var runnerInstances = GetStateMachineRunners(GetAllComponents(GetSceneObjects()));
            GetInstallersFromStateRunners(runnerInstances);
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
                installer.RegisterBindings(this);
            }

        }
        //1. get scene objects (all)
        //2. get components (all)
        //3. get StateRunners from components (all)
        //4. Iterate, get installers

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

        private List<object> GetStateMachineRunners(List<MonoBehaviour> sourceList)
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

        private void GetInstallersFromStateRunners(List<object> stateRunnerInstances)
        {
            foreach (var runnerInstance in stateRunnerInstances)
            {
                var fields = runnerInstance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                foreach (var fieldInfo in fields)
                {
                    var attribute = fieldInfo.GetCustomAttribute<Attributes.InstallerContainer>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    var installers = fieldInfo.GetValue(runnerInstance) as List<Installer>;
                    Install(installers);
                }
            }
        }
        #endregion

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
    }
    
    //todo: extract
    [Serializable]
    public class InstallerEntity
    {
        public string Id;
        public object ObjectInstance;
    }
}