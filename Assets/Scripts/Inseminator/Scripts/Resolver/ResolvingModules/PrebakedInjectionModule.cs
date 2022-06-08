namespace Inseminator.Scripts.Resolver.ResolvingModules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Data.Baking;
    using Helpers;
    using ReflectionBaking;
    using UnityEngine;

    public class PrebakedInjectionModule : ResolvingModule
    {
        #region Private Variables
        protected ReflectionBakingData bakingData;
        protected InseminatorDependencyResolver dependencyResolver;
        protected MemberInfoExtractor memberInfoExtractor = new MemberInfoExtractor();
        #endregion

        #region Public API
        public override void Run(InseminatorDependencyResolver dependencyResolver, object sourceObject)
        {
            this.dependencyResolver = dependencyResolver;
            
            bakingData = ReflectionBaker.Instance.BakingData;
            BakedDataLookup(sourceObject);
        }
        #endregion

        #region Private Methods
        protected virtual void BakedDataLookup(object sourceObject)
        {
            if (sourceObject == null)
            {
                //Debug.LogError($"Failed to start lookup");
                return;
            }
            //Debug.LogError($"Starting lookup for {sourceObject.GetType().GetNiceName()}");
            if (!bakingData.BakedInjectableFields.TryGetValue(sourceObject.GetType(), out var fieldBakingDatas))
            {
                //Debug.LogError($"Can't find baked fields for {sourceObject.GetType().GetNiceName()}");
                
                ResolveAndRunBakedMethods(sourceObject);
                ResolveNested(ref sourceObject);
                return;
            }
            foreach (var fieldBakingData in fieldBakingDatas)
            {
                var memberInfo = memberInfoExtractor.GetMember(fieldBakingData.MemberType, fieldBakingData.MemberName,
                    sourceObject, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
                if (memberInfo == null)
                {
                    Debug.LogError($"Failed to get member: {fieldBakingData.MemberName}");
                    continue;
                }
                var instance = ResolveSingleDependency(memberInfo.GetUnderlyingType(), fieldBakingData.Attribute?.InstanceId);
                if(preventOverridingExistingValues && instance == null) continue;
                memberInfo.SetValue(sourceObject, instance);
            }
            ResolveAndRunBakedMethods(sourceObject);

            ResolveNested(ref sourceObject);
        }
        
        protected virtual void ResolveNested(ref object parentInstance)
        {
            //Debug.LogError($"Trying to resolve nested {parentInstance.GetType().GetNiceName()}");

            if (parentInstance == null)
            {
                return;
            }
            if (!bakingData.BakedSurrogateFields.TryGetValue(parentInstance.GetType(),
                out var surrogateFieldBakingDatas))
            {
                //Debug.LogError($"Cannot get surrogate entry for {parentInstance.GetType().GetNiceName()}");
                return;
            }

            foreach (var surrogateFieldBakingData in surrogateFieldBakingDatas)
            {
                var memberInfo = memberInfoExtractor.GetMember(surrogateFieldBakingData.MemberType,
                    surrogateFieldBakingData.MemberName, parentInstance,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if (memberInfo == null)
                {
                    //Debug.LogError($"Failed to get member: {surrogateFieldBakingData.MemberName}");
                    continue;
                }
                var nestedInstance = memberInfo.GetValue(parentInstance);
                if (surrogateFieldBakingData.Attribute.ForceInitialization)
                {
                    nestedInstance = Activator.CreateInstance(memberInfo.GetUnderlyingType());
                    if(nestedInstance == null)
                    {
                        //Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    memberInfo.SetValue(parentInstance, nestedInstance);
                }
                    
                //BakedDataLookup(nestedInstance);
                dependencyResolver.ResolveDependencies(ref nestedInstance);
                if (memberInfo.GetUnderlyingType().IsValueType)
                {
                    memberInfo.SetValue(parentInstance, nestedInstance);
                }
            }
        }
        
        protected virtual object ResolveSingleDependency(Type targetType, object instanceId = null)
        {
            if (!dependencyResolver.RegisteredDependencies.TryGetValue(targetType, out var dependency))
            {
                if(logErrors)
                {
                    Debug.LogError($"Cannot get dependency instance for {targetType.Name} | {targetType}", this);
                }
                return default;
            }
            if (instanceId == null)
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }

        protected virtual void ResolveAndRunBakedMethods(object sourceObject)
        {
            var targetType = sourceObject.GetType();
            List<object> resolvedParameters = new List<object>();
            if (!bakingData.BakedMethods.TryGetValue(targetType, 
                out var bakedMethods))
            {
                //Debug.LogError($"No bake methods for {sourceObject.GetType().GetNiceName()}");
                return;
            }
            foreach (var bakedMethod in bakedMethods)
            {
                resolvedParameters.Clear();
                int paramIndex = 0;
                foreach (var paramValue in bakedMethod.ParameterTypes.Select(parameterType 
                    => ResolveSingleDependency(parameterType, bakedMethod.Attribute.ParamIds[paramIndex])))
                {
                    if (paramValue == null)
                    {
                        if(logErrors)
                        {
                            Debug.LogError($"No value for param {bakedMethod.Attribute.ParamIds[paramIndex]}");
                        }
                        paramIndex++;
                        continue;
                    }
                    resolvedParameters.Add(paramValue);
                    paramIndex++;
                }
                Debug.Log($"Trying to run injection method {bakedMethod.MemberName}");
                MethodsHelper.RunMethod(sourceObject, bakedMethod.MemberName, resolvedParameters);
            }
        }
        #endregion
    }
}