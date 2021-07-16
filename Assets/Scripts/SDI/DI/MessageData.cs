namespace SDI.DI
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Data/MessageData")]
    public class MessageData : ScriptableObject
    {
        [field: SerializeField, BoxGroup("Message"), HideLabel, MultiLineProperty(5)]
        public string Message { get; private set; }
    }
}