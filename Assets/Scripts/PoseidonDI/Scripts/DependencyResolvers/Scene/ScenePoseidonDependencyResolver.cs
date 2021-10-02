namespace PoseidonDI.Scripts.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ScenePoseidonDependencyResolver : PoseidonDependencyResolver
    {
        #region Public API
        public void ResolveExternalGameObject(ref GameObject externalInstance)
        {
            var components = PoseidonHelpers.GetAllComponents(new List<GameObject>() {externalInstance});
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
            var sceneObjects = PoseidonHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
                typeof(PoseidonDependencyResolver), 
                typeof(PoseidonInstaller)
            }, gameObject.scene);
            var sceneComponents = PoseidonHelpers.GetAllComponents(sceneObjects);

            foreach (var sceneComponent in sceneComponents)
            {
                var instance = (object)sceneComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        
        #region Installing
        protected override void Install(List<PoseidonInstaller> installers)
        {
            base.Install(installers);
            //add self to dependencies
            registeredDependencies.Add(typeof(ScenePoseidonDependencyResolver), new List<InstallerEntity>
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