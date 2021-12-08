﻿namespace Inseminator.Scripts.Factory
{
    using DependencyResolvers.GameObject;
    using Resolver;
    using UnityEngine;

    public class InseminatorMonoFactory : MonoBehaviour
    {
        #region Private Variables
        [InseminatorAttributes.Injectable] private InseminatorDependencyResolver sceneDependencyResolver;
        #endregion
        #region Public API
        public virtual T Create<T>(T templateObject, Transform parent = null) where T : Component
        {
            templateObject.gameObject.SetActive(false);
            
            var objectInstance = Instantiate(templateObject, parent);
            var instanceGameObject = objectInstance.gameObject;

            var gameObjectResolver = CheckForGameObjectContext(instanceGameObject);
            if (gameObjectResolver != null)
            {
                gameObjectResolver.InitializeResolver();
                objectInstance.gameObject.SetActive(true);
                return objectInstance;
            }
            
            sceneDependencyResolver.ResolveExternalGameObject(ref instanceGameObject);
            
            templateObject.gameObject.SetActive(true);
            objectInstance.gameObject.SetActive(true);
            
            return objectInstance;
        }
        #endregion

        #region Private Methods
        private GameObjectDependencyResolver CheckForGameObjectContext(GameObject sourceObject)
        {
            return sourceObject.GetComponent<GameObjectDependencyResolver>();
        }
        #endregion
    }
}