namespace Demo.API.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;

    public class HttpClientHelpers
    {
        public static HttpContent GetPostBody(Dictionary<string, string> parameters)
        {
            var formatted = parameters.Select(x => x.Key + "=" + x.Value);
            return new StringContent(string.Join("&", formatted), Encoding.UTF8, "application/x-www-form-urlencoded");
        }
    }
}