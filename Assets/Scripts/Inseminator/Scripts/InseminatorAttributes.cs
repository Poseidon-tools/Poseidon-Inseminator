namespace Inseminator.Scripts
{
    using System;

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
    }
}