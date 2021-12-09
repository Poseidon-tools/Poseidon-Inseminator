namespace Inseminator.Scripts.InseminatorUtilities
{
    using Data.Baking;
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [CreateAssetMenu(menuName = "Inseminator/ReflectionBaking/Test")]
    public class BakingTest : ScriptableObject
    {
        private ReflectionBaker reflectionBaker = new ReflectionBaker();

        [Button]
        private void BakeCurrentScene()
        {
            var scene = SceneManager.GetActiveScene();
            reflectionBaker.BakeScene(scene);
        }
        
        [Button]
        private void LoadBakingData()
        {
            ReflectionBakingData bakingData = reflectionBaker.LoadBakedData();
            foreach (var bakingDataBakedInjectableField in bakingData.BakedInjectableFields)
            {
                Debug.Log($"{bakingDataBakedInjectableField.Key.Name}: {bakingDataBakedInjectableField.Value.Count}");
            }
        }
    }
}