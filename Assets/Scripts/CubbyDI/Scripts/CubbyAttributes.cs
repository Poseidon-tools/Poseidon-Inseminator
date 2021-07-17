namespace CubbyDI.Scripts
{
    using System;

    public class CubbyAttributes
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