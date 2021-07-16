namespace SDI.DI.DependencyResolvers.ScriptableObject
{
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Utils;

    public class ScriptableObjectDependencyResolver : DependencyResolver
    {
        #region Inspector
        [SerializeField, BoxGroup("ScriptableObjects"), InfoBox("Hit refresh button to refresh values.", InfoMessageType.Info)]
        [HideLabel]
        private List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();
        #endregion

        #region Resolving
        protected override void GetTargetObjects()
        {
            foreach (var scriptableObject in scriptableObjects)
            {
                var instance = (object)scriptableObject;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        #region Editor
#if UNITY_EDITOR
        [BoxGroup("ScriptableObjects"), Button(ButtonSizes.Large)]
        private void RefreshScriptableObjects()
        {
            scriptableObjects = AssetsUtils.GetAllInstances<ScriptableObject>().ToList();
        }
#endif
        #endregion
    }
}