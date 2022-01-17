namespace Inseminator.Scripts.Resolver.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Unity.VisualScripting;
    using UnityEngine;

    public static class MethodsExtractor
    {
        #region Private Variables
        private static InseminatorDependencyResolver dependencyResolver;
        #endregion
        #region Public API
        public static void ResolveMethods(object sourceObject, InseminatorDependencyResolver resolver)
        {
            dependencyResolver = resolver;
            ResolveAndRun(FilterMethodsByAttribute(GetMethods(sourceObject)), sourceObject);
        }

        public static List<(MethodInfo, InseminatorAttributes.InseminateMethod)> GetInseminationMethods(object sourceObject)
        {
            var filteredMethods = FilterMethodsByAttribute(GetMethods(sourceObject));
            var resultList = new List<(MethodInfo, InseminatorAttributes.InseminateMethod)>();
            foreach (var filteredMethod in filteredMethods)
            {
                resultList.Add((filteredMethod, filteredMethod.GetAttribute<InseminatorAttributes.InseminateMethod>()));
            }

            return resultList;
        }
        #endregion

        #region Private Methods
        private static MethodInfo[] GetMethods(object sourceObject)
        {
            return sourceObject.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static List<MethodInfo> FilterMethodsByAttribute(MethodInfo[] methodInfos)
        {
            return methodInfos.Where(methodInfo => methodInfo.IsDefined(typeof(InseminatorAttributes.InseminateMethod))).ToList();
        }

        private static void ResolveAndRun(List<MethodInfo> filteredMethods, object methodOwner)
        {
            foreach (var methodInfo in filteredMethods)
            {
                var attr = methodInfo.GetCustomAttribute<InseminatorAttributes.InseminateMethod>();
                if (attr == null)
                {
                    continue;
                }

                var methodParams = methodInfo.GetParameters();
                if (methodParams.Length == 0)
                {
                    continue;
                }
                List<object> paramInjectingValues = new List<object>();
                int paramIndex = 0;
                foreach (var parameterInfo in methodParams)
                {
                    var paramValue = dependencyResolver.GetValueForType(parameterInfo.ParameterType, attr.ParamIds[paramIndex]);
                    if (paramValue == null)

                    {
                        paramIndex++;
                        continue;
                    }
                    paramInjectingValues.Add(paramValue);
                    paramIndex++;
                }

                if (methodParams.Length != paramInjectingValues.Count)
                {
                    return;
                }
                methodInfo.Invoke(methodOwner, paramInjectingValues.ToArray());
            }
        }
        #endregion
    }
}