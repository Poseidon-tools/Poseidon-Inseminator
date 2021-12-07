namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using System;

    [Serializable]
    public abstract class ResolvingModule
    {
        public abstract void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject);
    }
}