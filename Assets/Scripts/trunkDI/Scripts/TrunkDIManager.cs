namespace trunkDI.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class TrunkDIManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField, BoxGroup("Resolvers")]
        private List<TrunkDependencyResolver> resolvers = new List<TrunkDependencyResolver>();
        #endregion

        #region Unity Methods
        private void Awake()
        {
            foreach (var dependencyResolver in resolvers)
            {
                dependencyResolver.InitializeResolver();
            }
        }
        #endregion
        
        #region Editor Button
        [Button("Refresh resolvers")]
        private void Refresh()
        {
            resolvers = FindObjectsOfType<TrunkDependencyResolver>().ToList();
        }
        #endregion
    }
}