using System;
using System.IO;
using System.Reflection;
using ChallengeBoard.Email.Models;
using RazorEngine;

namespace ChallengeBoard.Email
{
    public static class EmailFactory
    {
        public static string ParseTemplate<T>(T model, EmailType emailType)  where T : IEmailModel
        {
            var templatePath = GetTemplatePath(emailType);

            string content;
            using (var reader = new StreamReader(templatePath))
            {
                content = reader.ReadToEnd();
            }

            return Razor.Parse(content, model);
        }

        private static string GetTemplatePath(EmailType emailType)
        {

            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Email\\Templates",
                                       string.Format("{0}.cshtml", emailType));
            return (path);
            //var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            //var uri = new UriBuilder(codeBase);
            //var dir = new DirectoryInfo(Uri.UnescapeDataString(uri.Path));

            //return dir.Parent != null && dir.Parent != null
            //           ? Path.Combine(dir.Parent.FullName, "Email\\Templates\\", string.Format("{0}.cshtml", emailType))
            //           : string.Empty;
        }
    }
}