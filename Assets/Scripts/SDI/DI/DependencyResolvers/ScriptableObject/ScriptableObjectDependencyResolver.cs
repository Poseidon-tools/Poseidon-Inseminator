namespace SDI.DI.DependencyResolvers.ScriptableObject
{
    using System.Collections.Generic;
    using System.Linq;
    using Installers;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Utils;

    public class ScriptableObjectDependencyResolver : DependencyResolver
    {
        #region Inspector
        [SerializeField, BoxGroup("Declared Installers"), InfoBox("Remember to drag your installer to this list!", InfoMessageType.Warning)]
        [HideLabel]
        private List<Installer> declaredInstallers = new List<Installer>();
        [SerializeField, BoxGroup("ScriptableObjects"), InfoBox("Hit refresh button to refresh values.", InfoMessageType.Info)]
        [HideLabel]
        private List<ScriptableObject> scriptableObjects = new List<ScriptableObject>();
        #endregion

        #region Resolving
        private void ResolveScriptableObjects()
        {
            foreach (var scriptableObject in scriptableObjects)
            {
                var instance = (object)scriptableObject;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        
        #region Unity Methods
        private void Awake()
        {
            Install(declaredInstallers);
            ResolveScriptableObjects();
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