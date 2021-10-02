namespace PoseidonDI.Scripts.Installers
{
    using Resolver;

    public interface IPoseidonInstaller
    {
        void InstallBindings(PoseidonDependencyResolver poseidonDependencyResolver);
    }
}