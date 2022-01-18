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
        #endregion
    }
}