namespace PoseidonDI.Scripts.Example
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "DI/Example/ExampleSOInjection")]
    public class ExampleSOInjection : ScriptableObject
    {
        #region Inspector
        [SerializeField, PoseidonAttributes.Injectable]
        private MessageData messageData;
        #endregion
    }
}