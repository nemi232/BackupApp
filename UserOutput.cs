using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Octokit; // Assuming GitHub API is used
    
    namespace GitBackupApp{ 
        public  class UserOutput : IOutput
        {
            
        public  async void PrintRepos(GitHubClient github)
        {
            var repositories = await github.Repository.GetAllForCurrent();
            int index = 1;
            // Display repositories
            Console.WriteLine("Private Repositories:");
            foreach (var repo in repositories)
            {
                Console.WriteLine($"{index}. {repo.FullName}");
                index++;
            }
        }

        public  void PrintFiles(StringBuilder stringBuilder, string encryptedFileName){
            IEncryption encrypt = new Encryption();
            
            string allIssuesText = stringBuilder.ToString();

            // Generate random key and IV
            byte[] key = encrypt.GenerateRandomKey(256); 
            byte[] iv = encrypt.GenerateRandomIV();

            // Encrypt the issues text
            byte[] encryptedData = encrypt.Encrypt(allIssuesText, key, iv);

            // Write encrypted data to file
            File.WriteAllBytes(encryptedFileName, encryptedData);

            Console.WriteLine($"Issues encrypted and saved to {encryptedFileName}");

            AskForDecryption(encryptedFileName, key, iv);
        }
        public void AskForDecryption(string encryptedFileName, byte[] key, byte[] iv)
        {
            IEncryption encrypt = new Encryption();
            Console.WriteLine("Do you want to decrypt the file? (Y/N)");
            string response = Console.ReadLine().ToUpper();

            if (response.Equals("Y") || response.Equals("YES"))
            {
                // Read encrypted data from file
                byte[] encryptedData = File.ReadAllBytes(encryptedFileName);

                // Decrypt the data
                string decryptedText = encrypt.Decrypt(encryptedData, key, iv);

                // Display decrypted content
                Console.WriteLine("\nDecrypted Issues:");
                Console.WriteLine(decryptedText);
            }
            else
            {
                Console.WriteLine("Exited");
            }
         }

    }



    }