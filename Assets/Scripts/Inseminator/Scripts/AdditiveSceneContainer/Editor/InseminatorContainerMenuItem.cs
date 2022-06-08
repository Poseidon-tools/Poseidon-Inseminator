namespace Inseminator.Scripts.AdditiveSceneContainer.Editor
{
#if UNITY_EDITOR
    using System.Linq;
    using Core.Utils.ScenesHelper;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class InseminatorContainerMenuItem
    {
        [MenuItem("GameObject/Inseminator/Convert scene to InseminatorAdditiveSceneContainer", false, priority = -100)]
        public static void CreateInseminatorContainer()
        {
            var container = new GameObject("InseminatorAdditiveSceneContainer");
            var containerComponent = container.AddComponent<InseminatorAdditiveSceneContainer>();
            containerComponent.ForceInitialize();
            EditorSceneManager.SaveOpenScenes();
        }
        [MenuItem("GameObject/Inseminator/Convert to standard scene layout", false, priority = -100)]
        public static void ConvertToStandardSceneLayout()
        {
            var rootObjects = ScenesHelper.GetSceneRootObjectsEditor(EditorSceneManager.GetActiveScene());

            var container =
                rootObjects.FirstOrDefault(r => r.GetComponent<InseminatorAdditiveSceneContainer>() != null);
            if(container == null) return;

            for (int i = container.transform.childCount - 1; i >= 0; i--)
            {
                var child = container.transform.GetChild(i);
                child.SetParent(null);
            }
            Object.DestroyImmediate(container.gameObject);
            EditorSceneManager.SaveOpenScenes();
        }
    }
#endif
    
}