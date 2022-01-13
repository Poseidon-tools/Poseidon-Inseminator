namespace Inseminator.Scripts.Data.Baking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    [Serializable]
    public sealed class InseminateFieldBakingData
    {
        public string MemberName;
        public MemberTypes MemberType;
        public InseminatorAttributes.Inseminate Attribute;
    }
    
    [Serializable]
    public sealed class SurrogateFieldBakingData
    {
        public string MemberName;
        public MemberTypes MemberType;
        public InseminatorAttributes.Surrogate Attribute;
    }
    
    [Serializable]
    public class ReflectionBakingData
    {
        #region Public Variables
        public Dictionary<Type, List<InseminateFieldBakingData>> BakedInjectableFields => bakedInjectableFields;
        public Dictionary<Type, List<SurrogateFieldBakingData>> BakedSurrogateFields => bakedSurrogateFields;

        public Dictionary<Type, List<string>> StateMachinesBaked = new Dictionary<Type, List<string>>();
        #endregion
        #region Private Variables
        private Dictionary<Type, List<InseminateFieldBakingData>> bakedInjectableFields = new Dictionary<Type, List<InseminateFieldBakingData>>();
        private Dictionary<Type, List<SurrogateFieldBakingData>> bakedSurrogateFields = new Dictionary<Type, List<SurrogateFieldBakingData>>();
        #endregion
    }
}