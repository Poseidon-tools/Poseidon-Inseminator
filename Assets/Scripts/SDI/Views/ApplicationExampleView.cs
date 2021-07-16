namespace SDI.Views
{
    using Core.ViewManager;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ApplicationExampleView : View
    {
        #region Inspector
        [field: SerializeField]
        public Button NextButton { get; private set; }
        [field: SerializeField]
        public TMP_Text MessageText { get; private set; }
        #endregion
    }
}