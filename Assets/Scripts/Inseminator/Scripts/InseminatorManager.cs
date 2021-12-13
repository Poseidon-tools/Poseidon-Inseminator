namespace Inseminator.Scripts
{
    using System.Collections.Generic;
    using System.Linq;
    using InseminatorUtilities;
    using Resolver;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class InseminatorManager : MonoBehaviour
    {
        #region Inspector
        [SerializeField, Header("Resolvers")]
        private List<InseminatorDependencyResolver> resolvers = new List<InseminatorDependencyResolver>();
        #endregion

        #region Unity Methods
        private void Awake()
        {
            ReflectionBaker.Instance.Initialize();
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
            resolvers = FindObjectsOfType<InseminatorDependencyResolver>().ToList();
        }
        #endregion
    }
}