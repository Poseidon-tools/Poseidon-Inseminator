namespace trunkDI.Scripts.DependencyResolvers.ScriptableObject
{
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Utils;

    public class ScriptableObjectTrunkDependencyResolver : TrunkDependencyResolver
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
                if (scriptableObject == null)
                {
                    // it could happen, because we have refs to ALL SO in project
                    // there is a chance that few of them won't be loaded/cached due to Odin serialization
                    continue;
                }
                var instance = (object)scriptableObject;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        #region Editor
        [BoxGroup("ScriptableObjects"), Button(ButtonSizes.Large)]
        private void RefreshScriptableObjects()
        {
        #if UNITY_EDITOR
            scriptableObjects = AssetsUtils.GetAllInstances<ScriptableObject>().ToList();
            #endif
        }
        #endregion
    }
}