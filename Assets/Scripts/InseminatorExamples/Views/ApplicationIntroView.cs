namespace InseminatorExamples.Views
{
    using Core.ViewManager;
    using UnityEngine;
    using UnityEngine.UI;

    public class ApplicationIntroView : View
    {
        #region Inspector
        [field: SerializeField]
        public Button NextButton { get; private set; }
        #endregion
    }
}