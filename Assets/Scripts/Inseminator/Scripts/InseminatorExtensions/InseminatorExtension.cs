namespace Core.Inseminator.Scripts.InseminatorExtensions
{
    using global::Inseminator.Scripts;
    using UnityEngine;

    public class InseminatorExtension : MonoBehaviour
    {
        public virtual void Enable(InseminatorManager inseminatorManager)
        {
            this.inseminatorManager = inseminatorManager;
        }

        public virtual void Disable()
        {
        }

        protected InseminatorManager inseminatorManager;
    }
}