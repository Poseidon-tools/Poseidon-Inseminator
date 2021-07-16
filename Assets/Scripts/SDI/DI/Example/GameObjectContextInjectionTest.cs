﻿namespace SDI.DI.Example
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    using Utils.ScenesHelper;

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
        #region Scene Test
        [Button]
        private async void LoadAdditionalScene()
        {
            SceneMaintainer sceneMaintainer = new SceneMaintainer();
            await sceneMaintainer.LoadScene("AdditionalSceneExample");
            Debug.Log("Loaded scene successfully.");
        }
        #endregion
    }
}