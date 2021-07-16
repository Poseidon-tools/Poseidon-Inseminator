namespace SDI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.StateMachine;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public static class DIHelpers
    {
        #region Unused (to be removed)
        public static List<GameObject> GetAllSceneObjects()
        {
            return Object.FindObjectsOfType<GameObject>().ToList();
        }
        public static List<GameObject> GetSceneObjectsExceptTypes(List<Type> excludedTypes)
        {
            List<GameObject> rootObjects = new List<GameObject>();
            Scene scene = SceneManager.GetActiveScene();
            scene.GetRootGameObjects(rootObjects);
            List<GameObject> sceneObjectsFiltered = new List<GameObject>();
            foreach (var rootObject in rootObjects)
            {
                GetChildrenWithCondition(rootObject, sceneObjectsFiltered, targetObject =>
                {
                    return excludedTypes.All(excludedType => targetObject.GetComponent(excludedType) == null);
                });
            }

            return sceneObjectsFiltered;
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

        #region Private Methods
        private static void GetChildrenWithCondition(GameObject parentObject, List<GameObject> outputList, Predicate<GameObject> condition)
        {
            if (!condition.Invoke(parentObject))
            {
                return;
            }
            int childCount = parentObject.transform.childCount;
            outputList.Add(parentObject);
            for (int i = 0; i < childCount; i++)
            {
                GetChildrenWithCondition(parentObject.transform.GetChild(i).gameObject, outputList, condition);
            }
        }
        #endregion
    }
}