namespace SDI.CubbyCustomResolvers.Modules
{
    using System;
    using System.Reflection;
    using Inseminator.Scripts.Data.Baking;
    using Poseidon.StateMachine;
    using UnityEngine;

    public class StateMachineResolver
    {
        #region Private Variables
        private Action<object> resolveMethod;
        #endregion
       
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
                if (elementType.GetGenericTypeDefinition() != typeof(State<>)) continue;
                //Debug.Log("Found states array!");
                var statesArray = propertyInfo.GetValue(stateManagerInstance) as object[];
                ResolveStates(statesArray);
            }
        }

        private void ResolveStates(object[] statesArray)
        {
            foreach (var state in statesArray)
            {
                var stateInstance = state;
                resolveMethod?.Invoke(stateInstance);
            }
        }

        #region Experimental
        public void ResolveStateMachines(object sourceObject, Action<object> resolveMethod)
        {
            this.resolveMethod = resolveMethod;
            var fields = sourceObject.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var targetType = fieldInfo.FieldType;
                if (!targetType.IsGenericType)
                {
                    //Debug.Log($"Field is not generic: {fieldInfo.Name}");
                    continue;
                }
               
                if (targetType.GetGenericTypeDefinition() != typeof(StateMachine<>))
                {
                    //Debug.Log($"Field is not StateMachine: {fieldInfo.Name}");
                    continue;
                }
                // this is StateManager<>
                //Debug.Log($"Found stateMachine: {fieldInfo.Name}");
                var stateManagerInstance = fieldInfo.GetValue(sourceObject);
                
                // get states from StateManager instance
                GetStates(stateManagerInstance);
            }
        }

        public void ResolveStateMachinesBaked(object sourceObject, Action<object> resolveMethod, ReflectionBakingData bakingData)
        {
            this.resolveMethod = resolveMethod;
            if (!bakingData.StateMachinesBaked.TryGetValue(sourceObject.GetType(), out var stateMachinesList)) return;
            foreach (var stateMachineFieldName in stateMachinesList)
            {
                var field = sourceObject.GetType().GetField(stateMachineFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field == null)
                {
                    Debug.LogError($"Cannot resolve {stateMachineFieldName} field in {sourceObject.GetType().Name}");
                    continue;
                }
                var stateManagerInstance = field.GetValue(sourceObject);
                
                // get states from StateManager instance
                GetStates(stateManagerInstance);
            }
        }
        #endregion
    }
}