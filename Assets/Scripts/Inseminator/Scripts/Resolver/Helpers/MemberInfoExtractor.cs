namespace Inseminator.Scripts.Resolver.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MemberInfoExtractor
    {
        #region Public API
        public List<MemberInfo> GetMembers(MemberTypes memberType, object instance, BindingFlags bindingFlags)
        {
            switch (memberType)
            {
                case MemberTypes.Property:
                    return instance.GetType().GetProperties(bindingFlags).Select(p => (MemberInfo)p).ToList();
                case MemberTypes.Field:
                    return instance.GetType().GetFields(bindingFlags).Select(p => (MemberInfo)p).ToList();
            }

            return null;
        }
        #endregion
    }
}