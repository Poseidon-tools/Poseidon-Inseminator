namespace Inseminator.Scripts
{
    using System.Collections.Generic;
    using Core.Inseminator.Scripts.InseminatorExtensions;
    using DependencyResolvers.Scene;
    using PersistentObjects;
    using ReflectionBaking;
    using Resolver;
    using UnityEngine;

    [DefaultExecutionOrder(-50)]
    public class InseminatorManager : MonoBehaviour
    {
        #region Public Variables
        public ResolverTreeNode ParentTreeNode { get; private set; }
        public static readonly InseminatorPersistentContainer PersistentContainer = new InseminatorPersistentContainer();
        #endregion

        #region Inspector
        [SerializeReference, SerializeField] private List<InseminatorExtension> extensions = new List<InseminatorExtension>();
        #endregion
        #region Private Methods
        private void Awake()
        {
            PersistentContainer.Initialize();
            PersistentObjectRegistry.Cleanup();

            ReflectionBaker.Instance.Initialize();
            
            InitializeExtensions();

            ParentTreeNode = BuildResolversTree(ParentTreeNode);
            ResolveTree(ParentTreeNode);
            
        }

        private void OnDestroy()
        {
            DisposeExtensions();
        }
        #endregion

        #region Public API
        public void ResolveTree(ResolverTreeNode node)
        {
            node.Resolver.InitializeResolver(node.Parent?.Resolver, PersistentContainer);
            foreach (var childNode in node.ChildNodes)
            {
                ResolveTree(childNode);
            }
        }
        #endregion
        
        #region Collecting resolvers
        public class ResolverTreeNode
        {
            public ResolverTreeNode Parent;
            public InseminatorDependencyResolver Resolver;
            public List<ResolverTreeNode> ChildNodes = new List<ResolverTreeNode>();
        }
        public ResolverTreeNode BuildResolversTree(ResolverTreeNode mainNode = null, InseminatorDependencyResolver resolver = null)
        {
            mainNode ??= new ResolverTreeNode()
            {
                Resolver = resolver == null ? FindObjectOfType<SceneDependencyResolver>() : resolver,
            };
            // get scene objects
            var sceneObjects = InseminatorHelpers.GetRootSceneObjects(mainNode.Resolver.gameObject.scene);

            foreach (var sceneObject in sceneObjects)
            {
                SearchForResolver(sceneObject.transform, mainNode);
            }
            return mainNode;
        }

        private ResolverTreeNode AddNode(ResolverTreeNode parentNode, InseminatorDependencyResolver currentResolver)
        {
            var node = new ResolverTreeNode()
            {
                Resolver = currentResolver,
                ChildNodes = new List<ResolverTreeNode>(),
                Parent =  parentNode
            };
            if (node.Resolver == parentNode.Resolver) return parentNode;
            parentNode.ChildNodes.Add(node);
            return node;

        }

        public void AddNodeTreeToRootNodeTree(ResolverTreeNode nodeTreeHead)
        {
            if(ParentTreeNode == null) return;
            if(nodeTreeHead.Resolver == ParentTreeNode.Resolver) return;
            ParentTreeNode.ChildNodes.Add(nodeTreeHead);
        }

        private void SearchForResolver(Transform target, ResolverTreeNode parentNode)
        {
            //Debug.Log($"Searching in {target.name}, parent: {parentNode.Resolver.name}");
            var childCount = target.childCount;
            InseminatorDependencyResolver resolver = null;
            resolver = target.GetComponent<InseminatorDependencyResolver>();
            ResolverTreeNode newParentNode = parentNode;
            if (resolver != null && !(resolver is SceneDependencyResolver))
            {
                newParentNode = AddNode(parentNode, resolver);
            }
            for (int i = 0; i < childCount; i++)
            {
                var child = target.GetChild(i);
                resolver = child.GetComponent<InseminatorDependencyResolver>();
                if (resolver is SceneDependencyResolver)
                {
                    continue;
                }
                if (resolver == null)
                {
                    SearchForResolver(child, newParentNode);
                    continue;
                }
                newParentNode = AddNode(parentNode, resolver);
                SearchForResolver(child, newParentNode);
            }
        }
        #endregion

        #region Private Methods
        private void InitializeExtensions()
        {
            foreach (var extension in extensions)
            {
                extension.Enable(this);
            }
        }
        private void DisposeExtensions()
        {
            foreach (var extension in extensions)
            {
                extension.Disable();
            }
        }
        #endregion
    }
}