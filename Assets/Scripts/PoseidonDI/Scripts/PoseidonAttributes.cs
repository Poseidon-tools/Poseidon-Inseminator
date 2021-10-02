namespace PoseidonDI.Scripts
{
    using System;

    public class PoseidonAttributes
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