namespace Poseidon.Example
{
    using StateMachine;
    using States;
    using UnityEngine;

    public class StateMachineExample : MonoBehaviour
    {
        private readonly StateMachine<ExampleStateType> exampleStateMachine = new StateMachine<ExampleStateType>(
            new State<ExampleStateType>[]
            {
                new ExampleIdleState(), 
                new ExampleUpdatedState()
            },
            ExampleStateType.ExampleUpdated);

        private void OnEnable()
        {
            exampleStateMachine.Run();
        }

        private void OnDisable()
        {
            exampleStateMachine.Dispose();
        }
    }
}
