namespace SDI.DI.Factory
{
    using DependencyResolvers.Scene;
    using UnityEngine;

    public class DIFactory : MonoBehaviour
    {
        #region Private Variables
        [Attributes.Injectable] private SceneDependencyResolver sceneDependencyResolver;
        #endregion
        #region Public API
        public virtual T Create<T>(T templateObject, Transform parent = null) where T : Component
        {
            templateObject.gameObject.SetActive(false);
            var objectInstance = Instantiate(templateObject, parent);
            var instanceGameObject = objectInstance.gameObject;
            sceneDependencyResolver.ResolveExternalGameObject(ref instanceGameObject);
            templateObject.gameObject.SetActive(true);
            objectInstance.gameObject.SetActive(true);
            return objectInstance;
        }
        #endregion
        
    }
}