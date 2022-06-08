namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using System;
    using System.Linq;
    using UnityEngine;

    public class PersistentContainerResolvingModule : PrebakedInjectionModule
    {
        #region Overrides

        protected override object ResolveSingleDependency(Type targetType, object instanceId = null)
        {
            if (dependencyResolver.PersistentContainer == null)
                return default;
            
            if (!dependencyResolver.PersistentContainer.RegisteredDependencies.TryGetValue(targetType, out var dependency))
            {
                if(logErrors)
                {
                    Debug.LogError(
                        $"Cannot get dependency instance for {targetType.Name} | {targetType} in persistent container",
                        this);
                }
                return default;
            }
            if (instanceId == null)
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }

        #endregion
    }
}