namespace Core.Utils.ScenesHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using global::Utils;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.SceneManagement;
    using Object = UnityEngine.Object;

    public static class SceneMaintainer
    {
        #region Events
        public static event Action<Scene, List<GameObject>> OnSceneLoaded;
        public static event Action<Scene> OnSceneUnloaded;
        #endregion
        
        #region Private Variables
        private static Scene currentLoadedScene;
        private static readonly Stack<Scene> activeScenesStack = new Stack<Scene>();
        private static EventSystem cachedEventSystem;
        #endregion

        #region Public Methods
        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {
            var components = ScenesHelper.GetComponentsOnScene<EventSystem>();
            if(components == null || components.Count == 0)
            {
                Debug.LogError("NO EVENT SYSTEM FOUND");
                return;
            }
            cachedEventSystem = components[0];
        }

        public static async UniTask LoadScene(string sceneName, Action<List<GameObject>> onLoadedCallback = null)
        {
            if(sceneName.IsNullOrEmpty())
            {
                Debug.LogError("Cannot load scene: invalid build index.");
                return;
            }

            // unload previous scene if current scene is valid
            if(currentLoadedScene.buildIndex >= 0)
            {
                var asyncOperation =
                    SceneManager.UnloadSceneAsync(currentLoadedScene,
                                                 UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
                if(asyncOperation != null)
                {
                    await asyncOperation.ToUniTask();
                }
            }

            activeScenesStack.Push(SceneManager.GetActiveScene());
            // load new scene
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            // assign current scene from buildIndex
            int sceneBuildIndex = SceneManager.GetSceneByName(sceneName).buildIndex;
            currentLoadedScene = GetCurrentScene(sceneBuildIndex);
            

            SceneManager.SetActiveScene(currentLoadedScene);
            // get and return root objects if callback isn't null
            var rootObjects = GetSceneRootObjects(sceneBuildIndex);

            RemoveDuplicateEventSystem(rootObjects);

            onLoadedCallback?.Invoke(rootObjects);
            OnSceneLoaded?.Invoke(currentLoadedScene, rootObjects);
        }

        private static void RemoveDuplicateEventSystem(List<GameObject> loadedSceneRootObjs)
        {
            var components = new List<EventSystem>();
            foreach (var sceneRootObj in loadedSceneRootObjs)
            {
                SeekForEventSystem(sceneRootObj, ref components);
            }
            if(components.Count == 0 ) return;

            for(int i = components.Count - 1; i >= 0; i--)
            {
                if(cachedEventSystem == components[i]) continue;
                Object.Destroy(components[i].gameObject);
            }
        }
        
        public static async UniTask UnloadScene(string sceneName, Action onUnloadCallback = null)
        {
            var unloadedScene = currentLoadedScene;
            await SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects).ToUniTask();
            if(activeScenesStack.Count > 0)
            {
                SceneManager.SetActiveScene(activeScenesStack.Pop());
            }
            onUnloadCallback?.Invoke();
            OnSceneUnloaded?.Invoke(unloadedScene);
        }

        public static T GetComponentInRootObjects<T>() where T : Component
        {
            var result = new List<GameObject>(currentLoadedScene.GetRootGameObjects());
            foreach(var gameObject in result)
            {
                gameObject.TryGetComponent(out T component);
                if(component != null)
                {
                    return component;
                }
            }
            return default;
        }

        
        #endregion

        #region Private Methods

        private static Scene GetCurrentScene(int buildIndex)
        {
            for(int i = 0; i < SceneManager.sceneCount; i++)
            {
                var currentScene = SceneManager.GetSceneAt(i);
                if(currentScene.buildIndex != buildIndex) continue;
                return currentScene;
            }
            return default;
        }
        
        private static List<GameObject> GetSceneRootObjects(int buildIndex)
        {
            var result = new List<GameObject>();
            result.AddRange(currentLoadedScene.GetRootGameObjects());
            return result;
        }
        
        private static void SeekForEventSystem(GameObject sourceObject, ref List<EventSystem> result)
        {
            result.AddRange(sourceObject.GetComponents<EventSystem>().ToList());
            for (int i = 0; i < sourceObject.transform.childCount; i++)
            {
                SeekForEventSystem(sourceObject.transform.GetChild(i).gameObject, ref result);
            }
        }
        
        #endregion
    }
}