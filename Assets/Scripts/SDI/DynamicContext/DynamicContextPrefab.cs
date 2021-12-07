namespace SDI.DynamicContext
{
    using System;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Poseidon.StateMachine;
    using StateMachine;
    using StateMachine.States;
    using TMPro;
    using UnityEngine;

    public class DynamicContextPrefab : MonoBehaviour
    {
        [SerializeField] private TMP_Text statusRenderer;
        #region private variables
        // this one should come from game object context
        [InseminatorAttributes.Injectable] private ITextLogger textLogger;

        private StateMachine<DynamicContextStateType> stateMachine = new StateMachine<DynamicContextStateType>(
            new State<DynamicContextStateType>[]
            {
                new DynamicContextIdleState(),
                new DynamicContextRunningState()
            }, DynamicContextStateType.Idle);
        #endregion

        #region Unity Methods
        private void OnEnable()
        {
            textLogger.LogMessage("Dynamic Game object context working.", statusRenderer);
            stateMachine.Run();
        }

        private void OnDisable()
        {
            stateMachine.Dispose();
        }
        #endregion
    }
}