namespace Inseminator.Scripts.Factory
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class InseminatorMonoFactoryInstaller : InseminatorInstaller
    {
        #region Inspector
        [SerializeField] private InseminatorMonoFactory factory;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            factory.AssignResolver(inseminatorDependencyResolver);
            inseminatorDependencyResolver.Bind<InseminatorMonoFactory>(factory);
        }
        #endregion
    }
}