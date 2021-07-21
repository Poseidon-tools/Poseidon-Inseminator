namespace trunkDI.Scripts.Factory
{
    using DependencyResolvers.Scene;
    using UnityEngine;

    public class TrunkFactory : MonoBehaviour
    {
        #region Private Variables
        [TrunkAttributes.Injectable] private SceneTrunkDependencyResolver sceneTrunkDependencyResolver;
        #endregion
        #region Public API
        public virtual T Create<T>(T templateObject, Transform parent = null) where T : Component
        {
            templateObject.gameObject.SetActive(false);
            
            var objectInstance = Instantiate(templateObject, parent);
            var instanceGameObject = objectInstance.gameObject;
            sceneTrunkDependencyResolver.ResolveExternalGameObject(ref instanceGameObject);
            
            templateObject.gameObject.SetActive(true);
            objectInstance.gameObject.SetActive(true);
            
            return objectInstance;
        }
        #endregion
        
    }
}