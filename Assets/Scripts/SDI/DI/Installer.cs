namespace SDI.DI
{
    using System;
    using System.Collections.Generic;
    using Data;
    using JetBrains.Annotations;
    using UnityEngine;

    public abstract class Installer : MonoBehaviour, IInstaller
    {
        #region Installer API
        public Dictionary<Type, List<InstallerEntity>> InstallerBindings { get; set; }

        [UsedImplicitly]
        public virtual void CreateBindings()
        {
            InstallerBindings = new Dictionary<Type, List<InstallerEntity>>();
        }
        #endregion
    }
}