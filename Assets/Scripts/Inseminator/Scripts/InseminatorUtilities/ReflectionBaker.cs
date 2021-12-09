namespace Inseminator.Scripts.InseminatorUtilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using BakingModules;
    using Data.Baking;
    using Installers;
    using Newtonsoft.Json;
    using Resolver;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class ReflectionBaker
    {
        private InseminatorBakingModule[] bakingModules = 
        {
            new StandardBakingModule(),
            new StateMachineBakingModule()
        };

        private ReflectionBakingData bakingData;
        #region Public API
        public static ReflectionBakingData GetBakingData()
        {
            string mainDirPath = Path.Combine(Application.persistentDataPath, "Inseminator/ReflectionBaking");
            if (!Directory.Exists(mainDirPath))
            {
                return null;
            }

            var filename = "ReflectionBakingData.json";
            var fullPath = $"{mainDirPath}/{filename}";
            var json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<ReflectionBakingData>(json);
        }
        public void BakeSingle(ref object singleObject)
        {
            PerformBaking(ref singleObject, bakingData);
        }
        public void BakeScene(Scene scene)
        {
            var sceneObjects = InseminatorHelpers.GetSceneObjectsExceptTypes(new List<Type>(), scene);
            var components = InseminatorHelpers.GetComponentsExceptTypes(sceneObjects, new List<Type>()
            {
                typeof(InseminatorDependencyResolver),
                typeof(InseminatorInstaller)
            });

            bakingData = new ReflectionBakingData();
            foreach (var sceneComponent in components)
            {
                var instance = (object)sceneComponent;
                PerformBaking(ref instance, bakingData);
            }
            SerializeBakingData(bakingData);
        }
        
        private void PerformBaking(ref object objectInstance, ReflectionBakingData bakingData)
        {
            foreach (var bakingModule in bakingModules)
            {
                bakingModule.Run(ref objectInstance, bakingData, this);
            }
        }

        public ReflectionBakingData LoadBakedData()
        {
            string mainDirPath = Path.Combine(Application.persistentDataPath, "Inseminator/ReflectionBaking");
            if (!Directory.Exists(mainDirPath))
            {
                return null;
            }

            var filename = "ReflectionBakingData.json";
            var fullPath = $"{mainDirPath}/{filename}";
            return DeserializeBakingData(fullPath);
        }
        #endregion

        #region Private Methods
        
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

        #region Files
        private void SerializeBakingData(ReflectionBakingData bakingData)
        {
            var json = JsonConvert.SerializeObject(bakingData);
            Debug.Log(json);

            string mainDirPath = Path.Combine(Application.persistentDataPath, "Inseminator/ReflectionBaking");
            if (!Directory.Exists(mainDirPath))
            {
                Directory.CreateDirectory(mainDirPath);
            }

            string filename = "ReflectionBakingData.json";
            string fullPath = $"{mainDirPath}/{filename}";
            File.WriteAllText(fullPath, json);
        }

        private ReflectionBakingData DeserializeBakingData(string path)
        {
            var json = File.ReadAllText(path);
            ReflectionBakingData bakingData = JsonConvert.DeserializeObject<ReflectionBakingData>(json);
            return bakingData;
        }
        #endregion
    }
}