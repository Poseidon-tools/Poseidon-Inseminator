namespace SDI.DI
{
    using System;

    public class Attributes
    {
        public class Injectable: Attribute
        {
            public string InstanceId;
        }
        
        public class InstallerContainer: Attribute
        {
        }

        public class NestedInjectable : Attribute
        {
            public bool ForceInitialization = false;
        }
    }
}