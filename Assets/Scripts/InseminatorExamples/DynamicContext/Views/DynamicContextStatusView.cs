namespace InseminatorExamples.DynamicContext.Views
{
    using Core.ViewManager;
    using TMPro;
    using UnityEngine;

    public class DynamicContextStatusView : View
    {
        #region Inspector
        [field: SerializeField] public TMP_Text StatusRenderer { get; private set; } 
        #endregion
    }
}