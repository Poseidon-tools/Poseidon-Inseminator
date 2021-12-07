namespace Inseminator.Scripts.Example
{
    using Core.ViewManager;
    using UnityEngine;

    public class TestNestedModuleInjection 
    {
        #region Private Variables
        [InseminatorAttributes.Injectable] private ViewManager usedViewManager;
        [InseminatorAttributes.NestedInjectable] private NestedInNested nested = new NestedInNested();
        [InseminatorAttributes.NestedInjectable(ForceInitialization = true)] private NestedInNested nestedUninitialized;
        #endregion
        #region Public API
        public void Alert()
        {
            Debug.Log($"Used ViewManager is: {usedViewManager.name}", usedViewManager);
            nested.Alert();
            // variable was not initialized manually using ctor
            nestedUninitialized.Alert();
        }
        #endregion
    }


    public class NestedInNested
    {
        #region Private Variables
        [InseminatorAttributes.Injectable] private MessageData messageData;
        [InseminatorAttributes.NestedInjectable] private Nested3rdLevelTest deeperLevelTest = new Nested3rdLevelTest();
        #endregion
        #region Public API
        
        public void Alert()
        {
            Debug.Log($"Message: {messageData.Message}");
            deeperLevelTest.Alert();
        }
        #endregion
    }

    public struct Nested3rdLevelTest
    {
        #region Private Variables
        [InseminatorAttributes.Injectable] private MessageData messageData;
        [InseminatorAttributes.Injectable] private ViewManager viewManager;
        #endregion
        #region Public API
        
        public void Alert()
        {
            Debug.Log($"Struct: Message: {messageData.Message}");
            Debug.Log($"Struct: ViewManager: {viewManager.name}");
        }
        #endregion
    }
}