namespace SDI.DI.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "DI/Example/ExampleSOInjection")]
    public class ExampleSOInjection : ScriptableObject
    {
        #region Inspector
        [SerializeField, Attributes.Injectable]
        private MessageData messageData;
        #endregion
    }
}