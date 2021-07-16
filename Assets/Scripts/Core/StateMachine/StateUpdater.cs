namespace Core.StateMachine
{
    using System;
    using UnityEngine;

    public class StateUpdater : MonoBehaviour
    {
        #region Public Variables
        public event Action OnUpdated;

        public static StateUpdater Instance
        {
            get
            {
                if (instance != null) return instance;
                instance = new GameObject("[State Updater]", typeof(StateUpdater)).GetComponent<StateUpdater>();
                DontDestroyOnLoad(instance);

                return instance;
            }
        }
        #endregion
        
        #region Private Variables
        private static StateUpdater instance;
        #endregion
        
        #region Unity Methods
        private void Update()
        {
            OnUpdated?.Invoke();
        }
        #endregion
    }
}