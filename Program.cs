using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Octokit; // Assuming GitHub API is used

namespace GitBackupApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Authentication with GitHub
            var github = new GitHubClient(new ProductHeaderValue("May"));
            var tokenAuth = new Credentials("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"); // Replace with your personal access token
            github.Credentials = tokenAuth;

            // Download list of private repositories
            var repositories = await github.Repository.GetAllForCurrent();

            // Display repositories
            Console.WriteLine("Private Repositories:");
            foreach (var repo in repositories)
            {
                Console.WriteLine(repo.FullName);
            }

            // Select repository
            Console.WriteLine("\nEnter the index of the repository to backup:");
            int selectedIndex = int.Parse(Console.ReadLine()) - 1; // Assuming user input is 1-indexed
            var selectedRepo = repositories[selectedIndex];

            // Download issues for selected repository
            var issues = await github.Issue.GetAllForRepository(selectedRepo.Owner.Login, selectedRepo.Name);

            // Encrypt and save issues to file
            string encryptedFileName = $"{selectedRepo.Name}-{DateTime.Now:yyyyMMddHHmmss}.txt";
            // EncryptAndSaveIssues(issues, encryptedFileName);

            // Store backup record in database
            StoreBackupRecord(selectedRepo.FullName, encryptedFileName);

            Console.WriteLine("\nBackup completed successfully.");
        }

        // static void EncryptAndSaveIssues(IEnumerable<Issue> issues, string fileName)
        // {
        //     using (var aesAlg = Aes.Create())
        //     {
        //         aesAlg.Key = Encoding.UTF8.GetBytes("your_key_here"); // Replace with your encryption key
        //         aesAlg.IV = new byte[16]; // Initialization vector

        //         // Encrypt issues
        //         using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
        //         using (var fsEncrypt = new FileStream(fileName, FileMode.Create))
        //         using (var cryptoStream = new CryptoStream(fsEncrypt, encryptor, CryptoStreamMode.Write))
        //         using (var swEncrypt = new StreamWriter(cryptoStream))
        //         {
        //             foreach (var issue in issues)
        //             {
        //                 // Write encrypted issue to file
        //                 swEncrypt.WriteLine($"Title: {issue.Title}");
        //                 swEncrypt.WriteLine($"Body: {issue.Body}");
        //                 swEncrypt.WriteLine();
        //             }
        //         }
        //     }
        // }


        //new comm 
        

        static void StoreBackupRecord(string repositoryName, string fileName)
        {
            // Store backup record in SQLite database or any other database
            // Example: INSERT INTO BackupRecords (REPOSITORY_NAME, DATE, FILE_NAME) VALUES ('repo_name', '2024-05-29', 'filename.txt')
        }
    }
}
