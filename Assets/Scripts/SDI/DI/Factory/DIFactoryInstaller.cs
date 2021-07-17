namespace SDI.DI.Factory
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using UnityEngine;

    public class DIFactoryInstaller : Installer
    {
        #region Inspector
        [SerializeField] private DIFactory factory;
        #endregion
        
        #region Public Methods
        public override void CreateBindings()
        {
            InstallerBindings = new Dictionary<Type, List<InstallerEntity>>
            {
                {
                    typeof(DIFactory), new List<InstallerEntity>
                    {
                        new InstallerEntity
                        {
                            Id = "",
                            ObjectInstance = factory
                        }
                    }
                }
            };
        }
        #endregion
    }
}