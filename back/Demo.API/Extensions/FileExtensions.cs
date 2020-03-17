namespace Demo.API.Extensions
{
    using System.IO;

    public static class FileExtensions
    {
        public static string ReadFile(this string path)
        {
            string body;
            using (var reader = new StreamReader(path))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }
    }
}