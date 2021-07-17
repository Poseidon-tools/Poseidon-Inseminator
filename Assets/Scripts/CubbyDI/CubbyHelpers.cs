namespace CubbyDI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class CubbyHelpers
    {
        #region Helper methods
      
        public static List<GameObject> GetSceneObjectsExceptTypes(List<Type> excludedTypes, Scene scene)
        {
            List<GameObject> rootObjects = new List<GameObject>();
            
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