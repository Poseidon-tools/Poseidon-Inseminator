namespace Inseminator.Scripts.Resolver.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public class MethodsExtractor
    {
        #region Private Variables
        private readonly InseminatorDependencyResolver dependencyResolver;
        #endregion
        #region Public API
        public MethodsExtractor(InseminatorDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }
        public void ResolveMethods(object sourceObject)
        {
            ResolveAndRun(FilterMethodsByAttribute(GetMethods(sourceObject)), sourceObject);
        }
        #endregion

        #region Private Methods
        private MethodInfo[] GetMethods(object sourceObject)
        {
            return sourceObject.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private List<MethodInfo> FilterMethodsByAttribute(MethodInfo[] methodInfos)
        {
            return methodInfos.Where(methodInfo => methodInfo.IsDefined(typeof(InseminatorAttributes.InseminateMethod))).ToList();
        }

        private void ResolveAndRun(List<MethodInfo> filteredMethods, object methodOwner)
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