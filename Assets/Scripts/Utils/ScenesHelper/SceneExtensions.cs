namespace Core.Utils.ScenesHelper
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class SceneExtensions
    {
        public static List<T> GetComponentsInRootGameObjects<T>(this Scene scene) where T : Component
        {
            var resultList = new List<T>();
            foreach(var rootObject in scene.GetRootGameObjects())
            {
                resultList.AddRange(rootObject.GetComponentsInChildren<T>());
            }

            return resultList;
        }
    }
}
