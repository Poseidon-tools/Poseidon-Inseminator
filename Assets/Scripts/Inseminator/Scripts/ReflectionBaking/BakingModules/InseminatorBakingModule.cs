namespace Inseminator.Scripts.ReflectionBaking.BakingModules
{
    using Data.Baking;

    public abstract class InseminatorBakingModule
    {
        public abstract void Run(ref object sourceObject, ReflectionBakingData bakingData, ReflectionBaker reflectionBaker);
    }
}