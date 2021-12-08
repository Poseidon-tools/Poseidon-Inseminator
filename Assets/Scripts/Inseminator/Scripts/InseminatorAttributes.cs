namespace Inseminator.Scripts
{
    using System;

    public class InseminatorAttributes
    {
        public class Inseminate: Attribute
        {
            public string InstanceId;
        }
        public class Surrogate : Attribute
        {
            public bool ForceInitialization = false;
        }
    }
}