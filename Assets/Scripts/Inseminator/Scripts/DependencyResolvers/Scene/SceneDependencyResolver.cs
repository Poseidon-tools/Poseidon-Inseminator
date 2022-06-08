namespace Inseminator.Scripts.DependencyResolvers.Scene
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using PersistentObjects;
    using Resolver;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneDependencyResolver : InseminatorDependencyResolver
    {
        #region Protected Variables
        protected List<MonoBehaviour> sceneComponents;
        #endregion
        #region Resolving
        protected override void GetTargetObjects()
        {
            var sceneObjects = InseminatorHelpers.GetSceneObjectsExceptTypes(new List<Type>()
            {
            }, gameObject.scene);
            // also look for persistent objects moved to DontDestroyOnLoad section from considered scene
            sceneObjects.AddRange(FetchPersistentObjectsBySceneOrigin(gameObject.scene));
            
            var filteredObjects = FilterSceneObjectsByParent(sceneObjects);
            sceneComponents = InseminatorHelpers.GetComponentsExceptTypes(filteredObjects, new List<Type>()
            {
                typeof(InseminatorDependencyResolver), 
                typeof(InseminatorInstaller)
            });

            foreach (var sceneComponent in sceneComponents)
            {
                var instance = (object)sceneComponent;
                ResolveDependencies(ref instance);
            }
        }
        #endregion

        #region Helpers
        protected List<GameObject> FilterSceneObjectsByParent(List<GameObject> sourceList)
        {
            var result = new List<GameObject>();
            foreach (var sceneObject in sourceList)
            {
                var currentParent = sceneObject.transform.parent;
                if (currentParent == null)
                {
                    result.Add(sceneObject);
                    continue;
                }
                while (currentParent != null)
                {
                    var scopedDependencyResolver = currentParent.GetComponent<InseminatorDependencyResolver>();
                    if (scopedDependencyResolver != null)
                    {
                        if (gameObject == scopedDependencyResolver.gameObject)
                        {
                            result.Add(sceneObject);
                        }

                        break;
                    }
                    currentParent = currentParent.parent;
                    if (currentParent == null)
                    {
                        result.Add(sceneObject);
                    }
                }
            }

            return result;
        }
        
          
        private List<GameObject> FetchPersistentObjectsBySceneOrigin(Scene scene)
        {
            return PersistentObjectRegistry.GetPersistentObjectsBySceneOrigin(scene);
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