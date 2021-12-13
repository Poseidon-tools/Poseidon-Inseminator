namespace SDI.CustomResolvers.Modules
{
    using CubbyCustomResolvers.Modules;
    using Inseminator.Scripts.InseminatorUtilities;
    using Inseminator.Scripts.Resolver;
    using Inseminator.Scripts.Resolver.ResolvingModules;

    public class BakedSMResolvingModule : ResolvingModule
    {
        #region Private Variables
        private StateMachineResolver stateMachineResolver;
        private InseminatorDependencyResolver dependencyResolver;
        #endregion
        #region Public API
        public override void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject)
        {
            stateMachineResolver = new StateMachineResolver();
            this.dependencyResolver = dependencyResolver;
            stateMachineResolver.ResolveStateMachinesBaked(sourceObject, ResolveWithRefWrapper, ReflectionBaker.Instance.BakingData);
        }
        #endregion
        
        #region Helpers
        private void ResolveWithRefWrapper(object resolvedObject)
        {
            dependencyResolver.ResolveDependencies(ref resolvedObject);
        }
        #endregion
    }
}