namespace Inseminator.Scripts.ReflectionBaking.BakingModules
{
    using System;
    using System.Reflection;
    using Data.Baking;
    using UnityEngine;

    public class StandardBakingModule  : InseminatorBakingModule
    {
        public override void Run(ref object sourceObject, ReflectionBakingData bakingData, ReflectionBaker reflectionBaker)
        {
            GetFieldsForInseminate(ref sourceObject, bakingData);
        }
        
        private void GetFieldsForInseminate(ref object instanceObject, ReflectionBakingData bakingData)
        {
            var allInjectableFields = instanceObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in allInjectableFields)
            {
                if (!fieldInfo.IsDefined(typeof(InseminatorAttributes.Inseminate)))
                {
                    continue;
                }
                var inseminateAttr = fieldInfo.GetCustomAttribute<InseminatorAttributes.Inseminate>( false);
                ReflectionBaker.UpdateBakingDataWithField(bakingData, instanceObject, fieldInfo, inseminateAttr);
            }
            HandleSurrogates(ref instanceObject, bakingData);
        }
        
        private void HandleSurrogates(ref object parentInstance, ReflectionBakingData bakingData)
        {
            var fields = parentInstance.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var fieldInfo in fields)
            {
                if (!fieldInfo.IsDefined(typeof(InseminatorAttributes.Surrogate)))
                {
                    continue;
                }
                var surrogateAttr = fieldInfo.GetCustomAttribute<InseminatorAttributes.Surrogate>(false);
                if (surrogateAttr == null)
                {
                    continue;
                }

                ReflectionBaker.UpdateBakingDataWithSurrogate(bakingData, parentInstance, fieldInfo, surrogateAttr);
                
                var nestedInstance = fieldInfo.GetValue(parentInstance);
                
                if (surrogateAttr.ForceInitialization)
                {
                    nestedInstance = Activator.CreateInstance(fieldInfo.FieldType);
                    if(nestedInstance == null)
                    {
                        Debug.LogError("Cannot create DI instance of object.");
                        continue;
                    }
                    fieldInfo.SetValue(parentInstance, nestedInstance);
                }
                
                GetFieldsForInseminate(ref nestedInstance, bakingData);
            }
        }
    }
}