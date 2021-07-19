namespace CubbyDI.Scripts.Installers
{
    using Resolver;

    public interface IInstaller
    {
        void InstallBindings(DependencyResolver dependencyResolver);
    }
}