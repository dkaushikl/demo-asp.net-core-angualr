namespace Demo.API.Extensions
{
    using System;
    using System.Linq;

    public static class EmailSenderExtensions
    {
        public static string SetEmailTemplates(this object replaceObj, string emailTemplate)
        {
            return replaceObj == null
                       ? emailTemplate
                       : replaceObj.GetType().GetProperties().Aggregate(
                           emailTemplate,
                           (current, prop) => current.Replace(
                               $"{{{prop.Name}}}",
                               Convert.ToString(prop.GetValue(replaceObj, null))));
        }
    }
}