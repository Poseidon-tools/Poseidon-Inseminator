namespace PoseidonDI.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class PoseidonDIManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField, BoxGroup("Resolvers")]
        private List<PoseidonDependencyResolver> resolvers = new List<PoseidonDependencyResolver>();
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
            resolvers = FindObjectsOfType<PoseidonDependencyResolver>().ToList();
        }
        #endregion
    }
}