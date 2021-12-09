namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Data.Baking;
    using InseminatorUtilities;
    using UnityEngine;

    public class PrebakedInjectionModule : ResolvingModule
    {
        #region Private Variables
        private ReflectionBakingData bakingData;
        private InseminatorDependencyResolver dependencyResolver;
        #endregion
        #region Public API
        public override void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject)
        {
            this.dependencyResolver = dependencyResolver;
            
            bakingData = ReflectionBaker.GetBakingData();
            BakedDataLookup(sourceObject);
        }
        #endregion

        #region Private Methods
        private void BakedDataLookup(object sourceObject)
        {
            if (!bakingData.BakedInjectableFields.TryGetValue(sourceObject.GetType(), out var fieldBakingDatas)) return;
            foreach (var fieldBakingData in fieldBakingDatas)
            {
                var fieldInfo = sourceObject.GetType()
                    .GetField(fieldBakingData.FieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    continue;
                }
                var instance = ResolveSingleDependency(fieldInfo.FieldType, fieldBakingData.Attribute?.InstanceId);
                fieldInfo.SetValue(sourceObject, instance);
            }

            ResolveNested(ref sourceObject);
        }
        
        private void ResolveNested(ref object parentInstance)
        {
            if (!bakingData.BakedSurrogateFields.TryGetValue(parentInstance.GetType(),
                out var surrogateFieldBakingDatas))
            {
                return;
            }

            foreach (var surrogateFieldBakingData in surrogateFieldBakingDatas)
            {
                var fieldInfo = parentInstance.GetType()
                    .GetField(surrogateFieldBakingData.FieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (fieldInfo == null)
                {
                    continue;
                }
                var nestedInstance = fieldInfo.GetValue(parentInstance);
                if (surrogateFieldBakingData.Attribute.ForceInitialization)
                {
                    nestedInstance = Activator.CreateInstance(fieldInfo.FieldType);
                    if(nestedInstance == null)
                    {
                        Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
                    
                BakedDataLookup(nestedInstance);
                if (fieldInfo.FieldType.IsValueType)
                {
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
            }
        }
        
        private object ResolveSingleDependency(Type targetType, object instanceId = null)
        {
            if (!dependencyResolver.RegisteredDependencies.TryGetValue(targetType, out var dependency))
            {
                Debug.LogError($"Cannot get dependency instance for {targetType.Name} | {targetType}");
                return default;
            }
            if (instanceId == null)
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }
        #endregion
    }
}