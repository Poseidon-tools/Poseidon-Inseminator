namespace InseminatorExamples.DynamicContext
{
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Inseminator.Scripts.Factory;
    using Poseidon.StateMachine;
    using StateMachine;
    using StateMachine.States;
    using TMPro;
    using UnityEngine;

    public class DynamicContextPrefab : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusRenderer;
        [SerializeField] private DynamicContextNested nestedObject;
        #region private variables
        // this one should come from game object context
        [InseminatorAttributes.Inseminate] private ITextLogger textLogger;
        
        [InseminatorAttributes.Inseminate] private MessageData sceneMessage;
        [InseminatorAttributes.Inseminate] private InseminatorMonoFactory monoFactory;

        private StateMachine<DynamicContextStateType> stateMachine = new StateMachine<DynamicContextStateType>(
            new State<DynamicContextStateType>[]
            {
                new DynamicContextIdleState(),
                new DynamicContextRunningState()
            });
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            textLogger.LogMessage("Dynamic Game object context working.", statusRenderer);
            Debug.Log($"Dynamic context injected scene message: {sceneMessage.Message}");
            stateMachine.Run();

            Debug.Log(monoFactory == null);
            monoFactory.Create<DynamicContextNested>(nestedObject, transform);
        }

        private void OnDisable()
        {
            stateMachine.Dispose();
        }
        #endregion
    }
}