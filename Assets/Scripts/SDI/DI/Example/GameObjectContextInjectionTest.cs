namespace SDI.DI.Example
{
    using UnityEngine;

    public class GameObjectContextInjectionTest : MonoBehaviour
    {
        #region Private Variables
        [Attributes.Injectable] private MessageData messageData;
        #endregion
        #region Unity Methods
        private void Start()
        {
            Debug.Log($"Hey hey, it's {name} here! Injected message is: {messageData.name} | {messageData.Message}");
        }
        #endregion
    }
}