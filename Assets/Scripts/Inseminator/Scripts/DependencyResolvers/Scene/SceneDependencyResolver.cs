namespace Inseminator.Scripts.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class SceneDependencyResolver : PoseidonDependencyResolver
    {
        #region Protected Variables
        protected List<MonoBehaviour> sceneComponents;
        #endregion
        #region Public API
        public void ResolveExternalGameObject(ref GameObject externalInstance)
        {
            var components = InseminatorHelpers.GetAllComponents(new List<GameObject>() {externalInstance});
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
            var sceneObjects = InseminatorHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
                typeof(PoseidonDependencyResolver), 
                typeof(InseminatorInstaller)
            }, gameObject.scene);
            sceneComponents = InseminatorHelpers.GetAllComponents(sceneObjects);

            foreach (var sceneComponent in sceneComponents)
            {
                var instance = (object)sceneComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion
        
        #region Installing
        protected override void Install(List<InseminatorInstaller> installers)
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