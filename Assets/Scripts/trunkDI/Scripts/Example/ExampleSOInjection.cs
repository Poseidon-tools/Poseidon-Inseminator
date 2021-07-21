namespace trunkDI.Scripts.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "DI/Example/ExampleSOInjection")]
    public class ExampleSOInjection : ScriptableObject
    {
        #region Inspector
        [SerializeField, TrunkAttributes.Injectable]
        private MessageData messageData;
        #endregion
    }
}