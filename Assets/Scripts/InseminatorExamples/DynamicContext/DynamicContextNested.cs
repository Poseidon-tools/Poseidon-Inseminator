namespace InseminatorExamples.DynamicContext
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class DynamicContextNested : MonoBehaviour
    {
        [InseminatorAttributes.Inseminate(InstanceId = "Code")]
        [SerializeField, MultiLineProperty(5)] private string accessCode;

        [InseminatorAttributes.Inseminate(InstanceId = "SomeSystem")]
        [SerializeField] private GameObject someSystem;

        [InseminatorAttributes.Inseminate] private ViewManager viewManager;

        private void Start()
        {
            Debug.Log($"Access code: {accessCode}");
            Debug.Log($"Used system: {someSystem.name}", someSystem);
            Debug.Log($"Used VM: {viewManager.name}", viewManager);
        }
    }
}