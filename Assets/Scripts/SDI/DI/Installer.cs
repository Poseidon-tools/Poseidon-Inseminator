namespace SDI.DI
{
    using JetBrains.Annotations;
    using UnityEngine;

    public abstract class Installer : MonoBehaviour
    {
        #region Public API
        [UsedImplicitly]
        public virtual void RegisterBindings(SceneContainer sceneContainer)
        {
            //todo: install your dependencies
            // and don't worry about sceneContainer parameter, it will be self-injected
        }
        #endregion
    }
}