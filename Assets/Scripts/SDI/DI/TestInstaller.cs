namespace SDI.DI
{
    using Core.ViewManager;
    using UnityEngine;

    public class TestInstaller : Installer
    {
        #region Inspector
        [SerializeField] private ViewManager sceneViewManager;
        [SerializeField] private MessageData sampleMessage;
        #endregion
        #region Public Methods
        public override void RegisterBindings(SceneContainer sceneContainer)
        {
            sceneContainer.RegisterDependency(typeof(ITextLogger), new InstallerEntity
            {
                Id = "TestLogger",
                ObjectInstance = new TestLogger()
            });
            sceneContainer.RegisterDependency(typeof(ITextLogger), new InstallerEntity
            {
                Id = "GreenTextLogger",
                ObjectInstance = new GreenTextLogger()
            });
            
            sceneContainer.RegisterDependency(typeof(ITextLogger), new InstallerEntity
            {
                Id = "CustomLoggerRed60",
                ObjectInstance = new CustomLogger(Color.red, 60)
            });
            sceneContainer.RegisterDependency(typeof(ViewManager), new InstallerEntity
            {
                Id = "",
                ObjectInstance = sceneViewManager
            });
            
            sceneContainer.RegisterDependency(typeof(MessageData), new InstallerEntity
            {
                Id = "",
                ObjectInstance = sampleMessage
            });
        }
        #endregion
    }
}