namespace SDI.DI.Example.ExampleInstallers
{
    using System;
    using System.Collections.Generic;
    using Core.ViewManager;
    using Data;
    using Installers;
    using UnityEngine;

    public class ExampleSceneInstaller : Installer
    {
        #region Inspector
        [SerializeField] private ViewManager sceneViewManager;
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void CreateBindings()
        {
            InstallerBindings = new Dictionary<Type, List<InstallerEntity>>
            {
                {
                    typeof(ITextLogger), new List<InstallerEntity>
                    {
                        new InstallerEntity
                        {
                            Id = "TestLogger",
                            ObjectInstance = new TestLogger()
                        },
                        new InstallerEntity
                        {
                            Id = "GreenTextLogger",
                            ObjectInstance = new GreenTextLogger()
                        },
                        new InstallerEntity
                        {
                            Id = "CustomLoggerRed60",
                            ObjectInstance = new CustomLogger(Color.red, 60)
                        }
                    }
                },
                
                {
                    typeof(ViewManager), new List<InstallerEntity>
                    {
                        new InstallerEntity
                        {
                            Id = "",
                            ObjectInstance = sceneViewManager
                        }
                    }
                },
                
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