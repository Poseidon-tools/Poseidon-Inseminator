namespace SDI.DynamicContext
{
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using TMPro;
    using UnityEngine;

    public class DynamicContextPrefab : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusRenderer;
        #region private variables
        // this one should come from game object context
        [InseminatorAttributes.Injectable] private ITextLogger textLogger;
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            textLogger.LogMessage("Dynamic Game object context working.", statusRenderer);
        }
        #endregion
    }
}