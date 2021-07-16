namespace SDI.DI.Containers.SceneContainer.Modules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.StateMachine;

    public class StateMachineResolver
    {
        #region Private Variables
        private Action<object> resolveMethod;
        #endregion
        public void ResolveStateMachineDependencies(object stateMachineRunnerInstance, Action<object> resolveMethod)
        {
            this.resolveMethod = resolveMethod;
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
                resolveMethod?.Invoke(stateInstance);
            }
        }
    }
}