namespace Core.Utils.ScenesHelper
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class ScenesHelper
    {
        public static List<string> GetAvailableScenesRawList()
        {
            return GetScenesInBuild().ToList();
        }

        public static List<GameObject> GetCurrentSceneRootObjects()
        {
            var scene = SceneManager.GetActiveScene();
            var rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            return rootObjects;
        }
        
        public static List<GameObject> GetSceneRootObjects(int loadedSceneIndex)
        {
            var scene = SceneManager.GetSceneAt(loadedSceneIndex);
            var rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            return rootObjects;
        }

        public static List<T> GetComponentsOnSceneEditor<T>(Scene scene)
        {
            var rootObjects = GetSceneRootObjectsEditor(scene);
            var resultList = new List<T>();
            foreach(var rootObject in rootObjects)
            {
                resultList.AddRange(rootObject.GetComponentsInChildren<T>());
            }

            return resultList;
        }
        public static List<T> GetComponentsOnScene<T>(bool includeInactive = false)
        {
            var rootObjects = GetCurrentSceneRootObjects();
            var resultList = new List<T>();
            foreach(var rootObject in rootObjects)
            {
                resultList.AddRange(rootObject.GetComponentsInChildren<T>(includeInactive));
            }

            return resultList;
        }
        public static List<T> GetComponentsOnScene<T>(int loadedSceneIndex)
        {
            var rootObjects = GetSceneRootObjects(loadedSceneIndex);
            var resultList = new List<T>();
            foreach(var rootObject in rootObjects)
            {
                resultList.AddRange(rootObject.GetComponentsInChildren<T>());
            }

            return resultList;
        }

        #region Editor

        public static List<GameObject> GetSceneRootObjectsEditor(Scene scene)
        {
            var rootObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootObjects);
            return rootObjects;
        }
        
        #endregion


        private static string[] GetScenesInBuild()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;     
            string[] scenes = new string[sceneCount];
            for(int i = 0; i < sceneCount; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }

            return scenes;
        }
    }
}