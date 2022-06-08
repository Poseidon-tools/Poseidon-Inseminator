namespace Inseminator.Scripts.AdditiveSceneContainer
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Utils.ScenesHelper;
    using UnityEngine;

    [DefaultExecutionOrder(-60)]
    public class InseminatorAdditiveSceneContainer : MonoBehaviour
    {
        #region Private Variables
        private Dictionary<GameObject, bool> activeStatusCacheLookup = new Dictionary<GameObject, bool>();
        #endregion

        #region Public API
        public void EnableContainer()
        {
            foreach (var cacheStatusPair in activeStatusCacheLookup)
            {
                cacheStatusPair.Key.SetActive(cacheStatusPair.Value);
            }
        }

        public void ForceInitialize()
        {
#if UNITY_EDITOR
            var rootObjects = ScenesHelper.GetSceneRootObjectsEditor(gameObject.scene);
            foreach (var rootObject in rootObjects.Where(rootObject => rootObject != gameObject))
            {
                rootObject.transform.SetParent(gameObject.transform);
            }
#endif
        }
        #endregion
        #region Unity API
        private void Awake()
        {
            CacheAndDisableChildren();
        }
        #endregion

        #region Private Methods
        private void CacheAndDisableChildren()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (activeStatusCacheLookup.ContainsKey(child.gameObject))
                {
                    continue;
                }
                activeStatusCacheLookup.Add(child.gameObject, child.gameObject.activeSelf);
                child.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}