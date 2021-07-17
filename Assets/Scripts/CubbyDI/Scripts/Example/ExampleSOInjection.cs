namespace CubbyDI.Scripts.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "DI/Example/ExampleSOInjection")]
    public class ExampleSOInjection : ScriptableObject
    {
        #region Inspector
        [SerializeField, CubbyAttributes.Injectable]
        private MessageData messageData;
        #endregion
    }
}