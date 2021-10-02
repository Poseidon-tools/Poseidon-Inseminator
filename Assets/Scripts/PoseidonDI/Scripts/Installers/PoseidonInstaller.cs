namespace PoseidonDI.Scripts.Installers
{
    using JetBrains.Annotations;
    using Resolver;
    using UnityEngine;

    public abstract class PoseidonInstaller : MonoBehaviour, IPoseidonInstaller
    {
        #region Installer API
        [UsedImplicitly]
        public abstract void InstallBindings(PoseidonDependencyResolver poseidonDependencyResolver);
        #endregion
    }
}