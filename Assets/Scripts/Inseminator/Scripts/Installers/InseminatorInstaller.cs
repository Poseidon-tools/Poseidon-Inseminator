namespace Inseminator.Scripts.Installers
{
    using JetBrains.Annotations;
    using Resolver;
    using UnityEngine;

    public abstract class InseminatorInstaller : MonoBehaviour, IInseminatorInstaller
    {
        #region Public Variables
        [field: SerializeField, Tooltip("Allow for injecting values from parent resolvers.")]
        public bool AllowForParentInsemination { get; private set; } = false;
        #endregion
        #region Installer API
        [UsedImplicitly]
        public abstract void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver);
        protected T ResolveInParent<T>(InseminatorDependencyResolver parentResolver) where T : Object
        {
            var searchedObject =
                (T)InseminatorHelpers.TryResolveInParentHierarchy<T>(parentResolver);
            if (searchedObject != null) return searchedObject;
            Debug.LogError("Can't find matching ViewManager in any of parent resolvers");
            return null;
        }
        #endregion
    }
}