namespace SDI.Views
{
    using Core.ViewManager;
    using CubbyDI.Example;
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