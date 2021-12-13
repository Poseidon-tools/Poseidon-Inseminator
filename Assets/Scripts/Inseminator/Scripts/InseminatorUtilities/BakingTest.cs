namespace Inseminator.Scripts.InseminatorUtilities
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Inseminator/ReflectionBaking/Test")]
    public class BakingTest : ScriptableObject
    {
        [Button(ButtonSizes.Gigantic), InfoBox("THIS IS HEAVY", InfoMessageType.Warning)]
        private void BakeAll()
        {
            ReflectionBaker.Instance.BakeAll();
        }
    }
}