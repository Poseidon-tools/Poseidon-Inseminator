namespace SDI.CustomResolvers.Modules
{
    using System;
    using CubbyCustomResolvers.Modules;
    using Inseminator.Scripts.Resolver;
    using Inseminator.Scripts.Resolver.ResolvingModules;

    [Serializable]
    public class StateMachineResolvingModule : ResolvingModule
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
            stateMachineResolver.ResolveStateMachines(sourceObject, ResolveWithRefWrapper);
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