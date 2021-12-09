namespace Inseminator.Scripts.InseminatorUtilities.BakingModules
{
    using System.Reflection;
    using Data.Baking;
    using Poseidon.StateMachine;
    using UnityEngine;

    public class StateMachineBakingModule : InseminatorBakingModule
    {
        private ReflectionBaker reflectionBaker;
        public override void Run(ref object sourceObject, ReflectionBakingData bakingData, ReflectionBaker reflectionBaker)
        {
            this.reflectionBaker = reflectionBaker;
            ResolveStateMachines(sourceObject, bakingData);
        }
        #region State Machine Helpers
        private void GetStates(object stateManagerInstance, ReflectionBakingData bakingData)
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
                ResolveStates(statesArray, bakingData);
            }
        }

        private void ResolveStates(object[] statesArray, ReflectionBakingData bakingData)
        {
            if (statesArray.Length == 0)
            {
                Debug.LogError("passed uninitialized array!");
                return;
            }
            foreach (var state in statesArray)
            {
                var stateInstance = state;
                reflectionBaker.BakeSingle(ref stateInstance);
            }
        }

        #region Experimental
        public void ResolveStateMachines(object sourceObject, ReflectionBakingData bakingData)
        {
            var fields = sourceObject.GetType()
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var targetType = fieldInfo.FieldType;
                if (!targetType.IsGenericType)
                {
                    continue;
                }
               
                if (targetType.GetGenericTypeDefinition() != typeof(StateMachine<>))
                {
                    continue;
                }
                // this is StateManager<>
                var stateManagerInstance = fieldInfo.GetValue(sourceObject);
                
                // get states from StateManager instance
                GetStates(stateManagerInstance, bakingData);
            }
        }
        #endregion
        
        #endregion
    }
}