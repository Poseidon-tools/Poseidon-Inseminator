namespace Inseminator.Scripts
{
    using System;

    public class InseminatorAttributes
    {
        public class Injectable: Attribute
        {
            public string InstanceId;
        }
        public class NestedInjectable : Attribute
        {
            public bool ForceInitialization = false;
        }
    }
}