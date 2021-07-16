﻿namespace SDI.DI.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Data;
    using Installers;
    using UnityEngine;
    using Utils;

    public abstract class DependencyResolver : MonoBehaviour
    {
        #region Private Variables
        protected Dictionary<Type, List<InstallerEntity>> registeredDependencies = new Dictionary<Type, List<InstallerEntity>>();
        #endregion
        #region Resolving Core
        protected virtual void ResolveNested(ref object parentInstance)
        {
            var fields = parentInstance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                var nestedInjectAttribute = fieldInfo.GetCustomAttribute<Attributes.NestedInjectable>();
                if (nestedInjectAttribute == null)
                {
                    continue;
                }
                
                var nestedInstance = fieldInfo.GetValue(parentInstance);
                if (nestedInjectAttribute.ForceInitialization)
                {
                    nestedInstance = TryForceInitializeInstance(fieldInfo.FieldType);
                    if(nestedInstance == null)
                    {
                        Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
                    
                ResolveDependencies(ref nestedInstance);
                if (fieldInfo.FieldType.IsValueType)
                {
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
            }
        }
        protected virtual void ResolveDependencies(ref object instanceObject)
        {
            var allInjectableFields = instanceObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var fieldInfo in allInjectableFields)
            {
                var injectAttribute = fieldInfo.GetCustomAttribute(typeof(Attributes.Injectable), true);
                if (injectAttribute == null)
                {
                    continue;
                }

                if (!(injectAttribute is Attributes.Injectable injectable)) continue;
                var instance = ResolveSingleDependency(fieldInfo.FieldType, injectable.InstanceId);
                fieldInfo.SetValue(instanceObject, instance);
            }
            ResolveNested(ref instanceObject);
        }

        protected virtual object ResolveSingleDependency(Type targetType, string instanceId = "")
        {
            if (!registeredDependencies.TryGetValue(targetType, out var dependency)) return default;
            if (instanceId.IsNullOrEmpty())
            {
                return dependency[0].ObjectInstance;
            }

            var matchingInstance = dependency.FirstOrDefault(instance => instance.Id.Equals(instanceId));
            return matchingInstance?.ObjectInstance;
        }
        #endregion
        
        #region Helpers
        protected virtual object TryForceInitializeInstance(Type type)
        {
            return Activator.CreateInstance(type);
        }
        #endregion
        
        #region Installing
        protected virtual void InstallDependency(Type targetType, InstallerEntity installerEntity)
        {
            if (registeredDependencies.TryGetValue(targetType, out var entry))
            {
                entry.Add(installerEntity);
                return;
            }
            registeredDependencies.Add(targetType, new List<InstallerEntity> {installerEntity});
        }

        protected virtual void Install(List<Installer> installers)
        {
            foreach (var installer in installers)
            {
                installer.CreateBindings();
                foreach (var installerBinding in installer.InstallerBindings)
                {
                    foreach (var installerEntity in installerBinding.Value)
                    {
                        InstallDependency(installerBinding.Key, installerEntity);
                    }
                }
            }
        }
        #endregion
       
    }
}