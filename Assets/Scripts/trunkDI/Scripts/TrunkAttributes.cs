namespace trunkDI.Scripts
{
    using System;

    public class TrunkAttributes
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