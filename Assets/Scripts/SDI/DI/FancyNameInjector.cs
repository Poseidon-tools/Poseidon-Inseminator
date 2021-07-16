﻿namespace SDI.DI
{
    using System.Collections.Generic;
    using System.Linq;
    using Resolver;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class FancyNameInjector : MonoBehaviour
    {
        #region Inspector
        [SerializeField, BoxGroup("Resolvers")]
        private List<DependencyResolver> resolvers = new List<DependencyResolver>();
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
            resolvers = FindObjectsOfType<DependencyResolver>().ToList();
        }
        #endregion
    }
}