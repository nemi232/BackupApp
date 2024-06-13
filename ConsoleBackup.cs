using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Octokit; // Assuming GitHub API is used


namespace GitBackupApp
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Interface.PrintRepos(ClentCredentials.ClientCred());
            // AppnedIssues();
            // Download list of private repositories
            var repositories = await ClentCredentials.ClientCred().Repository.GetAllForCurrent();

            // Select repository
            Console.WriteLine("\nEnter the index of the repository to backup:");
            int selectedIndex = int.Parse(Console.ReadLine()) - 1;

            var selectedRepo = repositories[selectedIndex];

            Console.WriteLine(selectedRepo.Name);

            // Download issues for selected repository
            var issues = await ClentCredentials.ClientCred().Issue.GetAllForRepository(selectedRepo.Owner.Login, selectedRepo.Name);

            // Encrypt and save issues to file            
            string encryptedFileName = $"{selectedRepo.Name}_{DateTime.Now:yyyy-MM-dd}.txt";

            //  all issues into a single string
            StringBuilder stringBuilder = new StringBuilder();
            int num = 1;
            if (issues != null && issues.Any())
            {
                foreach (var issue in issues)
                {
                    stringBuilder.AppendLine($"{num}. Title: {issue.Title}");
                    stringBuilder.AppendLine($"Body: {issue.Body}");
                    stringBuilder.AppendLine("------------");
                    num++;
                }
            }
            else
            {
                stringBuilder.AppendLine("No issues detected...");
            }
            Interface.PrintFiles(stringBuilder, encryptedFileName);            
        }
    }
}
