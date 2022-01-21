namespace SDI.DynamicContext
{
    using Core.ViewManager;
    using Inseminator.Scripts.Installers;
    using Inseminator.Scripts.Resolver;
    using UnityEngine;

    public class DynamicContextNestedInstaller : InseminatorInstaller
    {
        [SerializeField] private GameObject someObject;
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            inseminatorDependencyResolver.Bind(someObject, "SomeSystem");
            
            //this should come from parent-resolver
            var parentVM = ResolveInParent<ViewManager>(inseminatorDependencyResolver.Parent);
            inseminatorDependencyResolver.Bind(parentVM);

            // this should come from scene
            var accessCode = ResolveInParent<string>(inseminatorDependencyResolver.Parent, "AccessCode");
            inseminatorDependencyResolver.Bind(accessCode, "Code");
        }
    }
}