namespace VeraCustomTriage.Services
{
    public interface ITemplateWriter
    {
        string Write<T>(T model, string template);
    }

    public class TemplateWriter : ITemplateWriter
    {
        public string Write<T>(T model, string template)
        {
            var fields = typeof(T).GetProperties();
            foreach (var field in fields)
            {
                template = template.Replace("{" + field.Name + "}", field.GetValue(model).ToString());
            }
            return template;
        }
    }
}
