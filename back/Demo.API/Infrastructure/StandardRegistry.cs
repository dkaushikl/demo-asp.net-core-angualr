namespace Demo.API.Infrastructure
{
    using StructureMap;

    public class StandardRegistry : Registry
    {
        public StandardRegistry()
        {
            this.Scan(
                scan =>
                    {
                        scan.AssembliesFromApplicationBaseDirectory();
                        scan.WithDefaultConventions();
                    });
        }
    }
}