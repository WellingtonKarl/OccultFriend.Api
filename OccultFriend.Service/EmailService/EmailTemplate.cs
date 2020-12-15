using OccultFriend.Domain.DTO;
using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OccultFriend.Service.EmailService
{
    public class EmailTemplate : IEmailTemplate
    {
        // To-DO Melhorias!!!!
        private readonly string TemplatesFolder = Path.Combine(Directory.GetCurrentDirectory().Replace("\\OccultFriend.API\\", "\\"), "OccultFriend.Service", "Templates", "{0}.html");
        const string PropertyRegex = @"\{(.*?)\}";
        private Dictionary<string, string> Templates { get; set; } = new Dictionary<string, string>();

        public async Task<string> GenerateTemplateDrawEmail(string template, object model)
        {
            var templateString = await GetTemplateAsync(template);
            var matches = Regex.Matches(templateString, PropertyRegex);
            foreach (Match item in matches)
            {
                templateString = templateString.Replace(item.Value, GetPropValue(model, item.Groups[1].Value));
            }
            return templateString;
        }

        public string GenerateTextNamesDuplicate(HashSet<string> names)
        {
            string textNames = null;

            foreach (var name in names)
            {
                textNames = !string.IsNullOrEmpty(textNames) ? string.Concat(",", textNames, name,  ",") : string.Concat(textNames, name);
            }

            return textNames;
        }

        private async Task<string> GetTemplateAsync(string template)
        {
            if (!Templates.ContainsKey(template))
            {
                var templatePath = string.Format(TemplatesFolder, template);
                Templates.Add(template, await File.ReadAllTextAsync(templatePath));
            }
            return Templates[template];
        }

        private string GetPropValue(object obj, string propName)
        {
            string[] nameParts = propName.Split('.');
            if (nameParts.Length == 1)
            {
                return obj.GetType().GetProperty(propName).GetValue(obj)?.ToString();
            }

            foreach (string part in nameParts)
            {
                if (obj == null) { return null; }

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null) { return null; }

                obj = info.GetValue(obj, null);
            }
            return obj?.ToString();
        }
    }
}
