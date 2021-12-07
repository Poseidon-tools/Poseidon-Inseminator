namespace Poseidon.Example.States
{
    using StateMachine;
    using UnityEngine;

    public class ExampleUpdatedState :State<ExampleStateType>, IUpdatable
    {
        private const float TIME_TO_UPDATE = 1f;
        private float lastUpdatedTime;

        public override ExampleStateType StateType => ExampleStateType.ExampleUpdated;

        public override void OnEnter()
        {
            Debug.Log("Starting ExampleUpdatedState");
        }

        public override void OnExit()
        {
            Debug.Log("ExampleUpdatedState End");
        }

        public void OnUpdate()
        {
            if (Time.time - lastUpdatedTime > TIME_TO_UPDATE)
            {
                lastUpdatedTime = Time.time;
                Debug.Log("Updated in one second period");
            }
        }
    }
}