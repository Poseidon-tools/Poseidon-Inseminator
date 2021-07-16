namespace SDI.DI.DependencyResolvers.GameObject
{
    using System.Collections.Generic;
    using Resolver;
    using UnityEngine;

    public class GameObjectDependencyResolver : DependencyResolver
    {
        #region Resolving
        protected override void GetTargetObjects()
        {
            List<GameObject> childrenList = new List<GameObject>();
            GetChildren(gameObject, childrenList);
            var components = DIHelpers.GetAllComponents(childrenList);
            foreach (var component in components)
            {
                var instance = (object)component;
                ResolveDependencies(ref instance);
            }
        }

        private void GetChildren(GameObject parentObject, List<GameObject> outputList)
        {
            int childCount = parentObject.transform.childCount;
            outputList.Add(parentObject);
            for (int i = 0; i < childCount; i++)
            {
                GetChildren(parentObject.transform.GetChild(i).gameObject, outputList);
            }
        }
        #endregion
        #region Editor
#if UNITY_EDITOR
        
#endif
        #endregion
    }
}