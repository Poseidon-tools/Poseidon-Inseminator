namespace Inseminator.Scripts.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class SceneDependencyResolver : InseminatorDependencyResolver
    {
        #region Protected Variables
        protected List<MonoBehaviour> sceneComponents;
        #endregion
        #region Public API
        
        #endregion
        #region Resolving
        protected override void GetTargetObjects()
        {
            var sceneObjects = InseminatorHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
                typeof(InseminatorDependencyResolver), 
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
            registeredDependencies.Add(typeof(InseminatorDependencyResolver), new List<InstallerEntity>
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