namespace Inseminator.Scripts.Example
{
    using System.Reflection;
    using Core.ViewManager;
    using Factory;
    using Resolver.Helpers;
    using SDI.DynamicContext;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class MonoInjectable : MonoBehaviour
    {
        // example of property injection
        [field: SerializeField, InseminatorAttributes.Inseminate, PreviewField]
        public MessageData SceneScopeMessageData { get; private set; }
        
        [SerializeField] private DynamicContextPrefab dynamicContextPrefab;
        #region Private Variables
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;
        [InseminatorAttributes.Inseminate] private InseminatorMonoFactory monoFactory;
        #endregion
        #region Unity Methods
        private void Start()
        {
            Debug.Log($"Hey, it's ${name} here! I'm using properly injected ViewManager: {viewManager.name}");
            
            Invoke(nameof(InstantiateDynamicContextPrefab), 10f);
        }
        #endregion

        [Button]
        private void InstantiateDynamicContextPrefab()
        {
            monoFactory.Create<DynamicContextPrefab>(dynamicContextPrefab);
        }

        [Button]
        private void TestMemberExtractor()
        {
            var memberExtractor = new MemberInfoExtractor();
            var members = memberExtractor.GetMembers(MemberTypes.Property, this, BindingFlags.Public | BindingFlags.Instance);
            foreach (var memberInfo in members)
            {
                if (memberInfo == null)
                {
                    Debug.Log("!");
                    continue;
                }
                Debug.Log($"{memberInfo.MemberType} {memberInfo.Name}");
            }
        }
    }
}