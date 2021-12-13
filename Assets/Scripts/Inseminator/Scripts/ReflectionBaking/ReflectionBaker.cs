namespace Inseminator.Scripts.ReflectionBaking
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using BakingModules;
    using Data;
    using Data.Baking;
    using Installers;
    using Newtonsoft.Json;
    using Resolver;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using Utils;

    public class ReflectionBaker
    {
        #region Public Variables
        public static ReflectionBaker Instance => reflectionBaker ??= new ReflectionBaker();
        public ReflectionBakingData BakingData => bakingData;
        #endregion

        #region Private Variables
        private static ReflectionBaker reflectionBaker;
        private InseminatorBakingModule[] bakingModules = 
        {
            new StandardBakingModule(),
            new StateMachineBakingModule()
        };

        private ReflectionBakingData bakingData;

        private const string BAKING_DATA_PATH = "Inseminator/ReflectionBaking";
        private const string BAKING_DATA_FILENAME = "ReflectionBakingData.json";
        #endregion
        #region Public API
        public void Initialize()
        {
            bakingData = LoadBakedData();
        }
        public void BakeAll()
        {
            bakingData = new ReflectionBakingData();
            //todo: bake all scenes first
            Debug.Log("Attempt to bake all scenes.");
            LoadAllScenes();
            Debug.Log("Attempt to bake assets.");
            BakeFromAssets();
            //todo: bake SO
            
            SerializeBakingData(bakingData);
        }
        public void BakeSingle(ref object singleObject)
        {
            PerformBaking(ref singleObject, bakingData);
        }
        #endregion

        #region Private Methods
        private void BakeScene(Scene scene)
        {
            var sceneObjects = InseminatorHelpers.GetSceneObjectsExceptTypes(new List<Type>(), scene);
            var components = InseminatorHelpers.GetComponentsExceptTypes(sceneObjects, new List<Type>()
            {
                typeof(InseminatorDependencyResolver),
                typeof(InseminatorInstaller)
            });

            foreach (var sceneComponent in components)
            {
                var instance = (object)sceneComponent;
                PerformBaking(ref instance, bakingData);
            }
        }

        private void BakeFromAssets()
        {
            var settings = AssetsUtils.FindFirstAsset<InseminatorSettings>();
            var allProjectGO = AssetsUtils.FindPrefabs<Transform>(settings != null ? settings.BakeablePrefabsPaths.ToArray() : null).Select(t => t.gameObject).ToList();
            var allComponents = InseminatorHelpers.GetComponentsExceptTypes(allProjectGO, new List<Type>()
            {
                typeof(InseminatorDependencyResolver),
                typeof(InseminatorInstaller)
            });
            foreach (var component in allComponents)
            {
                var instance = (object)component;
                PerformBaking(ref instance, bakingData);
            }
            Debug.Log("Finished baking process for project types.");
        }
        
        private void PerformBaking(ref object objectInstance, ReflectionBakingData bakingData)
        {
            foreach (var bakingModule in bakingModules)
            {
                bakingModule.Run(ref objectInstance, bakingData, this);
            }
        }

        private ReflectionBakingData LoadBakedData()
        {
            var filename = BAKING_DATA_FILENAME;
            var fullPath = $"{BAKING_DATA_PATH}/{filename}".Replace(".json","");
            Debug.Log($"{fullPath}");
            var asset = Resources.Load<TextAsset>(fullPath);
            return DeserializeBakingData(asset.text);
        }
        #endregion

        #region Baking Data Helpers
        public static void UpdateBakingDataWithField(ReflectionBakingData bakingData, object instanceObject, FieldInfo fieldInfo, InseminatorAttributes.Inseminate inseminateAttr = null)
        {
            if (bakingData.BakedInjectableFields.TryGetValue(instanceObject.GetType(), out var fieldBakingDatas))
            {
                var existingField = fieldBakingDatas.Find(f =>
                    string.Compare(f.FieldName, fieldInfo.Name, StringComparison.Ordinal) == 0); 
                if (existingField != null)
                {
                    return;
                }
                fieldBakingDatas.Add(new InseminateFieldBakingData()
                {
                    Attribute =  inseminateAttr,
                    FieldName = fieldInfo.Name
                });
                return;
            }
            bakingData.BakedInjectableFields.Add(instanceObject.GetType(), new List<InseminateFieldBakingData>()
            {
                new InseminateFieldBakingData
                {
                    Attribute =  inseminateAttr,
                    FieldName = fieldInfo.Name
                }
            });
        }
        public static  void UpdateBakingDataWithSurrogate(ReflectionBakingData bakingData, object instanceObject, FieldInfo fieldInfo, InseminatorAttributes.Surrogate surrogateAttr = null)
        {
            if (bakingData.BakedSurrogateFields.TryGetValue(instanceObject.GetType(), out var fieldBakingDatas))
            {
                var existingField = fieldBakingDatas.Find(f =>
                    string.Compare(f.FieldName, fieldInfo.Name, StringComparison.Ordinal) == 0); 
                if (existingField != null)
                {
                    return;
                }
                fieldBakingDatas.Add(new SurrogateFieldBakingData()
                {
                    Attribute =  surrogateAttr,
                    FieldName = fieldInfo.Name
                });
                return;
            }
            bakingData.BakedSurrogateFields.Add(instanceObject.GetType(), new List<SurrogateFieldBakingData>()
            {
                new SurrogateFieldBakingData
                {
                    Attribute =  surrogateAttr,
                    FieldName = fieldInfo.Name
                }
            });
        }
        #endregion

        #region Scenes
        private void LoadAllScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var scene in scenes)
            {
                EditorSceneManager.OpenScene(scene.path);
                Debug.Log($"Baking scene {scene.path}");
                var tmpScene = EditorSceneManager.GetActiveScene();

                BakeScene(tmpScene);
            }

            EditorSceneManager.OpenScene(scenes[0].path);
        }
        #endregion

        #region Files
        private void SerializeBakingData(ReflectionBakingData bakingData)
        {
            var json = JsonConvert.SerializeObject(bakingData);

            string mainDirPath = Path.Combine(Application.dataPath, $"Resources/{BAKING_DATA_PATH}");
            if (!Directory.Exists(mainDirPath))
            {
                Directory.CreateDirectory(mainDirPath);
            }

            string filename = BAKING_DATA_FILENAME;
            string fullPath = $"{mainDirPath}/{filename}";
            File.WriteAllText(fullPath, json);
        }

        private ReflectionBakingData DeserializeBakingData(string json)
        {
            ReflectionBakingData bakingData = JsonConvert.DeserializeObject<ReflectionBakingData>(json);
            return bakingData;
        }
        #endregion
    }
}