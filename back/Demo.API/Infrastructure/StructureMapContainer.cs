namespace Demo.API.Infrastructure
{
    using StructureMap;

    public class StructureMapContainer : IStructureMapContainer
    {
        public StructureMapContainer(Container container)
        {
            this.Container = container;
        }

        public Container Container { get; }
    }
}