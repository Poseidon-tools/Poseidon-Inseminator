namespace trunkDI.Scripts.Factory
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class TrunkFactoryInstaller : TrunkInstaller
    {
        #region Inspector
        [SerializeField] private TrunkFactory factory;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(TrunkDependencyResolver trunkDependencyResolver)
        {
            trunkDependencyResolver.Bind<TrunkFactory>(factory);
        }
        #endregion
    }
}