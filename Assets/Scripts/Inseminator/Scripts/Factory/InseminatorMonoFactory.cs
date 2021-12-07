namespace Inseminator.Scripts.Factory
{
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
            sceneDependencyResolver.ResolveExternalGameObject(ref instanceGameObject);
            
            templateObject.gameObject.SetActive(true);
            objectInstance.gameObject.SetActive(true);
            
            return objectInstance;
        }
        #endregion
    }
}