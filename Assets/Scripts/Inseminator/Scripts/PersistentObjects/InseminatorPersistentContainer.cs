namespace Inseminator.Scripts.PersistentObjects
{
    using System;
    using System.Collections.Generic;
    using Data;

    public sealed class InseminatorPersistentContainer
    {
        #region Public Variables
        public Dictionary<Type, List<InstallerEntity>> RegisteredDependencies { get; private set; }
        #endregion

        #region Public API

        public void Initialize()
        {
            RegisteredDependencies = new Dictionary<Type, List<InstallerEntity>>();
        }
        
        public void InstallPersistentDependency(Type targetType, InstallerEntity installerEntity)
        {
            if (RegisteredDependencies.TryGetValue(targetType, out var entry))
            {
                entry.Add(installerEntity);
                return;
            }
            RegisteredDependencies.Add(targetType, new List<InstallerEntity> {installerEntity});
        }
        #endregion
    }
}