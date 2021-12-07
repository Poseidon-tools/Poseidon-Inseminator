namespace Poseidon.Example.States
{
    using StateMachine;
    public class ExampleIdleState : State<ExampleStateType>
    {
        public override ExampleStateType StateType => ExampleStateType.ExampleIdle;
    }
}