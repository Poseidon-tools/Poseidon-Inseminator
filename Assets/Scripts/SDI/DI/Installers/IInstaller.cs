namespace SDI.DI.Installers
{
    using System;
    using System.Collections.Generic;
    using Data;

    public interface IInstaller
    {
        Dictionary<Type, List<InstallerEntity>> InstallerBindings { get; set; }
        void CreateBindings();
    }
}