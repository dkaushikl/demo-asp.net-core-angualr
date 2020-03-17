namespace Demo.API.Infrastructure
{
    using StructureMap;

    public interface IStructureMapContainer
    {
        Container Container { get; }
    }
}