namespace SDI.DI.Example.ExampleInstallers
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using UnityEngine;

    public class ExampleGameObjectInstaller : Installer
    {
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void CreateBindings()
        {
            InstallerBindings = new Dictionary<Type, List<InstallerEntity>>
            {
                {
                    typeof(MessageData), new List<InstallerEntity>
                    {
                        new InstallerEntity
                        {
                            Id = "",
                            ObjectInstance = sampleMessage
                        }
                    }
                }
            };
        }
        #endregion
    }
}