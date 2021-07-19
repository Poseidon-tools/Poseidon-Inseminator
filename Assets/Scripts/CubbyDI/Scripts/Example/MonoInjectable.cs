﻿namespace CubbyDI.Scripts.Example
{
    using Core.ViewManager;
    using UnityEngine;

    public class MonoInjectable : MonoBehaviour
    {
        #region Private Variables
        [CubbyAttributes.Injectable] private ViewManager viewManager;
        #endregion
        #region Unity Methods
        private void Start()
        {
            Debug.Log($"Hey, it's ${name} here! I'm using properly injected ViewManager: {viewManager.name}");
        }
        #endregion
    }
}