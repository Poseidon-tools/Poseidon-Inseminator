namespace PoseidonDI.Scripts.Factory
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class PoseidonFactoryInstaller : PoseidonInstaller
    {
        #region Inspector
        [SerializeField] private PoseidonFactory factory;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(PoseidonDependencyResolver poseidonDependencyResolver)
        {
            poseidonDependencyResolver.Bind<PoseidonFactory>(factory);
        }
        #endregion
    }
}