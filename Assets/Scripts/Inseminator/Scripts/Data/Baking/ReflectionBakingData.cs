namespace Inseminator.Scripts.Data.Baking
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public sealed class InseminateFieldBakingData
    {
        public string FieldName;
        public InseminatorAttributes.Inseminate Attribute;
    }
    
    [Serializable]
    public sealed class SurrogateFieldBakingData
    {
        public string FieldName;
        public InseminatorAttributes.Surrogate Attribute;
    }
    
    [Serializable]
    public class ReflectionBakingData
    {
        #region Public Variables
        public Dictionary<Type, List<InseminateFieldBakingData>> BakedInjectableFields => bakedInjectableFields;
        public Dictionary<Type, List<SurrogateFieldBakingData>> BakedSurrogateFields => bakedSurrogateFields;
        #endregion
        #region Private Variables
        private Dictionary<Type, List<InseminateFieldBakingData>> bakedInjectableFields = new Dictionary<Type, List<InseminateFieldBakingData>>();
        private Dictionary<Type, List<SurrogateFieldBakingData>> bakedSurrogateFields = new Dictionary<Type, List<SurrogateFieldBakingData>>();
        #endregion
    }
}