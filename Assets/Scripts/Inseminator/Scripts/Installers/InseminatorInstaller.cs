namespace Inseminator.Scripts.Installers
{
    using JetBrains.Annotations;
    using Resolver;
    using UnityEngine;

    public abstract class InseminatorInstaller : MonoBehaviour, IInseminatorInstaller
    {
        #region Installer API
        [UsedImplicitly]
        public abstract void InstallBindings(PoseidonDependencyResolver poseidonDependencyResolver);
        #endregion
    }
}