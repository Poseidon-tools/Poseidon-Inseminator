namespace Inseminator.Scripts
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Newtonsoft.Json;

    public class InseminatorAttributes
    {
        public class Inseminate: Attribute
        {
            public object InstanceId;
        }
        public class Surrogate : Attribute
        {
            public bool ForceInitialization = false;
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class InseminateMethod : Attribute
        {
            public object[] ParamIds = new object[]{};

            public InseminateMethod(Type ownerType, [CallerMemberName] string callerName = null)
            {
                if (callerName == null) return;
                var method = ownerType.GetMethod(callerName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                if(method == null)
                {
                    return;
                }
                var args = method.GetParameters();
                ParamIds = new object[args.Length];
                for (var index = 0; index < args.Length; index++)
                {
                    ParamIds[index] = string.Empty;
                }
            }

            [JsonConstructor]
            public InseminateMethod()
            {
            }
            public InseminateMethod(object[] paramIds)
            {
                ParamIds = paramIds;
            }

            public InseminateMethod(int paramsAmount)
            {
                ParamIds = new object[paramsAmount];
                for (var i = 0; i < paramsAmount; i++)
                {
                    ParamIds[i] = string.Empty;
                }
            }
        }
    }
}