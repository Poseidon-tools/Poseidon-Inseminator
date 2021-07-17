namespace CubbyDI.Scripts.Factory
{
    using System;
    using System.Collections.Generic;
    using Data;
    using Installers;
    using UnityEngine;

    public class CubbyFactoryCubbyInstaller : CubbyInstaller
    {
        #region Inspector
        [SerializeField] private CubbyFactory factory;
        #endregion
        
        #region Public Methods
        public override void CreateBindings()
        {
            InstallerBindings = new Dictionary<Type, List<InstallerEntity>>
            {
                {
                    typeof(CubbyFactory), new List<InstallerEntity>
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