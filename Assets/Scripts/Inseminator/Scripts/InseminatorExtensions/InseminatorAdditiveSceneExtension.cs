namespace Core.Inseminator.Scripts.InseminatorExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Inseminator.Scripts;
    using global::Inseminator.Scripts.AdditiveSceneContainer;
    using global::Inseminator.Scripts.DependencyResolvers.Scene;
    using global::Inseminator.Scripts.PersistentObjects;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Utils.ScenesHelper;

    public class InseminatorAdditiveSceneExtension : InseminatorExtension
    {
        #region Public API
        public override void Enable(InseminatorManager inseminatorManager)
        {
            base.Enable(inseminatorManager);
            SceneMaintainer.OnSceneLoaded += OnNewSceneLoadedHandler;
            SceneMaintainer.OnSceneUnloaded += OnSceneUnloadedHandler;
        }

        private void OnSceneUnloadedHandler(Scene unloadedScene)
        {
        }

        private void OnNewSceneLoadedHandler(Scene loadedScene, List<GameObject> rootObjects)
        {
            Debug.Log($"Start building new scene tree... [{loadedScene.name}]");
            

            var container =
                rootObjects.FirstOrDefault(r => r.GetComponent<InseminatorAdditiveSceneContainer>() != null);
            if(container == null) return;

            rootObjects.Clear();

            GetAllChildObjects(container, rootObjects);
            
            var sceneResolverGameObject = rootObjects.FirstOrDefault(r => r.GetComponentInChildren<SceneDependencyResolver>() != null);
            Debug.Log($"Looking for scene resolver...");
            if(sceneResolverGameObject == null) return;
            var sceneResolver = sceneResolverGameObject.GetComponentInChildren<SceneDependencyResolver>();

            var newSceneNode = new InseminatorManager.ResolverTreeNode()
            {
                Parent = inseminatorManager.ParentTreeNode,
                Resolver = sceneResolver,
                ChildNodes = new List<InseminatorManager.ResolverTreeNode>()
            };

            newSceneNode = inseminatorManager.BuildResolversTree(newSceneNode, sceneResolver);
            
            Debug.Log($"Resolving node tree... {newSceneNode.Parent.Resolver.name}");
            inseminatorManager.ResolveTree(newSceneNode);
            
            Debug.Log($"Finished resolving node tree for new scene.");
            container.GetComponent<InseminatorAdditiveSceneContainer>().EnableContainer();
        }

        public override void Disable()
        {
            SceneMaintainer.OnSceneLoaded -= OnNewSceneLoadedHandler;
            SceneMaintainer.OnSceneUnloaded -= OnSceneUnloadedHandler;
        }
        #endregion

        #region Private Methods
        private void GetAllChildObjects(GameObject parent, List<GameObject> flatOutputList)
        {
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                if(!flatOutputList.Contains(parent.transform.GetChild(i).gameObject))
                {
                    flatOutputList.Add(parent.transform.GetChild(i).gameObject);
                }
                GetAllChildObjects(parent.transform.GetChild(i).gameObject, flatOutputList);
            }
        }
        #endregion
    }
}