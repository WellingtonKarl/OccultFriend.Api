using OccultFriend.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OccultFriend.Service.EmailService
{
    public class EmailTemplate : IEmailTemplate
    {
        // To-DO Melhorias!!!!

        #region Properties

        private static string PropertyRegex => @"\{(.*?)\}";
        private static string TemplatesFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "{0}.html");
        private Dictionary<string, string> Templates { get; set; } = new Dictionary<string, string>();

        #endregion

        #region Methods Publics

        public async Task<string> GenerateTemplateDrawEmail(string template, object viewModel)
        {
            var templateString = await GetTemplateAsync(template);
            var matches = Regex.Matches(templateString, PropertyRegex);
            foreach (Match item in matches)
            {
                templateString = templateString.Replace(item.Value, GetPropValue(viewModel, item.Groups[1].Value));
            }
            return templateString;
        }

        public string GenerateTextNamesDuplicate(HashSet<string> names)
        {
            var textNames = new StringBuilder();

            foreach (var name in names)
            {
                textNames.Append($"{name}, ");
            }

            return textNames.Length > 0 ? textNames.ToString().Remove(textNames.Length - 2) : textNames.ToString();
        }

        #endregion

        #region Methods Privates

        private async Task<string> GetTemplateAsync(string template)
        {
            if (!Templates.ContainsKey(template))
            {
                Templates.Add(template, await File.ReadAllTextAsync(TemplatesFolder));
            }
            return Templates[template];
        }

        private static string GetPropValue(object obj, string propName)
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

        #endregion
    }
}
