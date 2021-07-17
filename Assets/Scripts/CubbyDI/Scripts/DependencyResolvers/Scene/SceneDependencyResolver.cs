namespace CubbyDI.Scripts.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using Resolver;
    using UnityEngine;

    //todo: extract custom StateManager resolving logic, make derived class
    public class SceneDependencyResolver : DependencyResolver
    {
        #region Public API
        public void ResolveExternalGameObject(ref GameObject externalInstance)
        {
            var components = CubbyHelpers.GetAllComponents(new List<GameObject>() {externalInstance});
            foreach (var externalComponent in components)
            {
                var instance = (object)externalComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        #region Resolving
        protected override void GetTargetObjects()
        {
            var sceneObjects = CubbyHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
                typeof(DependencyResolver), 
                typeof(CubbyInstaller)
            }, gameObject.scene);
            var sceneComponents = CubbyHelpers.GetAllComponents(sceneObjects);

            foreach (var sceneComponent in sceneComponents)
            {
                var instance = (object)sceneComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        
        #region Installing
        protected override void Install(List<CubbyInstaller> installers)
        {
            base.Install(installers);
            //add self to dependencies
            registeredDependencies.Add(typeof(SceneDependencyResolver), new List<InstallerEntity>
            {
                new InstallerEntity
                {
                    Id = "",
                    ObjectInstance = this
                }
            });
        }
        #endregion
    }
    
}