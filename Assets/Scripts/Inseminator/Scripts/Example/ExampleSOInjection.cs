namespace Inseminator.Scripts.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "DI/Example/ExampleSOInjection")]
    public class ExampleSOInjection : ScriptableObject
    {
        #region Inspector
        [SerializeField, InseminatorAttributes.Injectable]
        private MessageData messageData;
        #endregion
    }
}