namespace SDI.CustomResolvers
{
    using CubbyCustomResolvers.Modules;
    using PoseidonDI.Scripts.DependencyResolvers.Scene;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class CustomScenePoseidonDependencyResolver : ScenePoseidonDependencyResolver
    {
        #region Private Variables
        private StateMachineResolver stateMachineResolver = new StateMachineResolver();
        #endregion
        #region Overrides
        public override void OnAfterGetObjects()
        {
            foreach (var sceneComponent in sceneComponents)
            {
                stateMachineResolver.ResolveStateMachines(sceneComponent, ResolveWithRefWrapper);
            }
        }
        #endregion
        
        #region Helpers
        private void ResolveWithRefWrapper(object resolvedObject)
        {
            ResolveDependencies(ref resolvedObject);
        }
        #endregion
    }
}