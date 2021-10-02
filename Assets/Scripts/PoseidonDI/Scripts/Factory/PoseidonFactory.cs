namespace PoseidonDI.Scripts.Factory
{
    using DependencyResolvers.Scene;
    using UnityEngine;

    public class PoseidonFactory : MonoBehaviour
    {
        #region Private Variables
        [PoseidonAttributes.Injectable] private ScenePoseidonDependencyResolver scenePoseidonDependencyResolver;
        #endregion
        #region Public API
        public virtual T Create<T>(T templateObject, Transform parent = null) where T : Component
        {
            templateObject.gameObject.SetActive(false);
            
            var objectInstance = Instantiate(templateObject, parent);
            var instanceGameObject = objectInstance.gameObject;
            scenePoseidonDependencyResolver.ResolveExternalGameObject(ref instanceGameObject);
            
            templateObject.gameObject.SetActive(true);
            objectInstance.gameObject.SetActive(true);
            
            return objectInstance;
        }
        #endregion
    }
}