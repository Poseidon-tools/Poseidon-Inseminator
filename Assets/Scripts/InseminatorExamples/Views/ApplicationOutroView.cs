﻿namespace InseminatorExamples.Views
{
    using Core.ViewManager;
    using Inseminator.Scripts.Example;
    using UnityEngine;
    using UnityEngine.UI;

    public class ApplicationOutroView : View
    {
        #region Inspector
        [field: SerializeField]
        public RectTransform ItemsContainer { get; private set; }
        
        [field: SerializeField]
        public Button SpawnButton { get; private set; }
        [field: SerializeField]
        public DynamicItemExample TemplateItem { get; private set; }
        #endregion
    }
}