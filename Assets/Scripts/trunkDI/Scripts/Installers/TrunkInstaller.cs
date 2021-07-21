namespace trunkDI.Scripts.Installers
{
    using JetBrains.Annotations;
    using Resolver;
    using UnityEngine;

    public abstract class TrunkInstaller : MonoBehaviour, ITrunkInstaller
    {
        #region Installer API
        [UsedImplicitly]
        public abstract void InstallBindings(TrunkDependencyResolver trunkDependencyResolver);
        #endregion
    }
}