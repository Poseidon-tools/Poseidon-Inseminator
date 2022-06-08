namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using UnityEngine;

    public abstract class ResolvingModule : MonoBehaviour
    {
        #region Inspector
        [SerializeField, Header("Safety")] protected bool preventOverridingExistingValues;
        [SerializeField, Header("Log")] protected bool logErrors;
        #endregion
        public abstract void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject);
    }
}