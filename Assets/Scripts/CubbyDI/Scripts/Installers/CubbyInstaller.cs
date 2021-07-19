namespace CubbyDI.Scripts.Installers
{
    using JetBrains.Annotations;
    using Resolver;
    using UnityEngine;

    public abstract class CubbyInstaller : MonoBehaviour, IInstaller
    {
        #region Installer API
        [UsedImplicitly]
        public abstract void InstallBindings(DependencyResolver dependencyResolver);
        #endregion
    }
}