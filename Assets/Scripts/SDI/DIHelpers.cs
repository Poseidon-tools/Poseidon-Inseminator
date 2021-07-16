namespace SDI
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.StateMachine;
    using UnityEngine;

    public static class DIHelpers
    {
        #region Unused (to be removed)
        public static List<GameObject> GetSceneObjects()
        {
            return Object.FindObjectsOfType<GameObject>().ToList();
        }
        public static List<MonoBehaviour> GetAllComponents(List<GameObject> sourceObjectsList)
        {
            List<MonoBehaviour> components = new List<MonoBehaviour>();
            foreach (var listItem in sourceObjectsList)
            {
                components.AddRange(listItem.GetComponents<MonoBehaviour>());
            }
            return components;
        }

        public static List<object> GetSceneStateMachineRunners(List<MonoBehaviour> sourceList)
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
    }
}