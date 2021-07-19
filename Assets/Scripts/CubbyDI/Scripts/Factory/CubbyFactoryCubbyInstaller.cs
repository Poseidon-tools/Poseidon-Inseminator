namespace CubbyDI.Scripts.Factory
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class CubbyFactoryCubbyInstaller : CubbyInstaller
    {
        #region Inspector
        [SerializeField] private CubbyFactory factory;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(DependencyResolver dependencyResolver)
        {
            dependencyResolver.Bind<CubbyFactory>(factory);
        }
        #endregion
    }
}