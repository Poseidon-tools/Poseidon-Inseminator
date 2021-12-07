namespace Inseminator.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class InseminatorManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField, Header("Resolvers")]
        private List<PoseidonDependencyResolver> resolvers = new List<PoseidonDependencyResolver>();
        #endregion

        #region Unity Methods
        private void Awake()
        {
            Refresh();
            foreach (var dependencyResolver in resolvers)
            {
                dependencyResolver.InitializeResolver();
            }
        }
        #endregion
        
        #region Editor Button
        private void Refresh()
        {
            resolvers = FindObjectsOfType<PoseidonDependencyResolver>().ToList();
        }
        #endregion
    }
}