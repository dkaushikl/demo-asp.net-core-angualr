namespace Demo.API.Infrastructure.TaskManagement
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    public class PerRequestMiddleware
    {
        private readonly IStructureMapContainer _container;

        private readonly RequestDelegate next;

        public PerRequestMiddleware(RequestDelegate next, IStructureMapContainer container)
        {
            this.next = next;
            this._container = container;
        }

        public async Task Invoke(HttpContext context)
        {
            this.BeginInvoke(context);
            await this.next.Invoke(context);
            this.EndInvoke(context);
        }

        private void BeginInvoke(HttpContext context)
        {
            using (var container = this._container.Container.GetNestedContainer())
            {
                foreach (var task in container.GetAllInstances<IRunOnEachRequest>()) task.Execute();
            }
        }

        private void EndInvoke(HttpContext context)
        {
            using (var container = this._container.Container.GetNestedContainer())
            {
                foreach (var task in container.GetAllInstances<IRunAfterEachRequest>()) task.Execute();
            }
        }
    }
}