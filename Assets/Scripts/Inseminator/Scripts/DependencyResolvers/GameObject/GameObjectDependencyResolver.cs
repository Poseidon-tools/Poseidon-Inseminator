namespace Inseminator.Scripts.DependencyResolvers.GameObject
{
    using System.Collections.Generic;
    using Data;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class GameObjectDependencyResolver : InseminatorDependencyResolver
    {
        #region Resolving
        protected override void GetTargetObjects()
        {
            List<GameObject> childrenList = new List<GameObject>();
            GetChildren(gameObject, childrenList);
            var components = InseminatorHelpers.GetAllComponents(childrenList);
            foreach (var component in components)
            {
                var instance = (object)component;
                ResolveDependencies(ref instance);
            }
        }

        protected override void Install(List<InseminatorInstaller> installers)
        {
            base.Install(installers);
            
            if(!registeredDependencies.ContainsKey(typeof(InseminatorDependencyResolver)))
            {
                registeredDependencies.Add(typeof(InseminatorDependencyResolver), new List<InstallerEntity>
                {
                    new InstallerEntity
                    {
                        Id = "",
                        ObjectInstance = this
                    }
                });
                return;
            }
            registeredDependencies[typeof(InseminatorDependencyResolver)].Add(new InstallerEntity
            {
                Id = "",
                ObjectInstance = this
            });
        }

        private void GetChildren(GameObject parentObject, List<GameObject> outputList)
        {
            var childCount = parentObject.transform.childCount;
            if(outputList.Count == 0)
            {
                outputList.Add(parentObject);
                for (int i = 0; i < childCount; i++)
                {
                    GetChildren(parentObject.transform.GetChild(i).gameObject, outputList);
                }
            }
            else
            {
                if (parentObject.GetComponent<InseminatorDependencyResolver>() != null) return;
                outputList.Add(parentObject);
                for (var i = 0; i < childCount; i++)
                {
                    GetChildren(parentObject.transform.GetChild(i).gameObject, outputList);
                }
            }
        }
        #endregion
    }
}