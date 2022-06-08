namespace Inseminator.Scripts.PersistentObjects
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class PersistentObjectRegistry
    {
        #region Public Variables
        private static readonly Dictionary<Scene, List<GameObject>> persistentObjectLookup =
            new Dictionary<Scene, List<GameObject>>();
        #endregion
        #region Public API
        public static void Register(GameObject gameObject)
        {
            if (persistentObjectLookup.TryGetValue(gameObject.scene, out var scenePersistentObjects))
            {
                if (scenePersistentObjects.Contains(gameObject))
                {
                    Object.Destroy(gameObject);
                    return;
                }
                scenePersistentObjects.Add(gameObject);
            }
            else
            {
                persistentObjectLookup.Add(gameObject.scene, new List<GameObject>(){gameObject});
            }
            gameObject.transform.SetParent(null);
            Object.DontDestroyOnLoad(gameObject);
        }

        public static List<GameObject> GetPersistentObjectsBySceneOrigin(Scene scene)
        {
            return persistentObjectLookup.TryGetValue(scene, out var scenePersistentObjects) ? scenePersistentObjects : new List<GameObject>();
        }

        public static void Cleanup()
        {
            persistentObjectLookup.Clear();
        }
        #endregion
    }
}