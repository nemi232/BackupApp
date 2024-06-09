using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Octokit; // Assuming GitHub API is used
using System.Data.SQLite;

namespace GitBackupApp
{
    public static class Program
    {

        public static async Task Main(string[] args)
        {
            // Authentication with GitHub
            var github = new GitHubClient(new ProductHeaderValue("May"));
            var tokenAuth = new Credentials("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");
            github.Credentials = tokenAuth;

            // Download list of private repositories
            var repositories = await github.Repository.GetAllForCurrent();
            int index = 1;
            // Display repositories
            Console.WriteLine("Private Repositories:");
            foreach (var repo in repositories)
            {
                Console.WriteLine($"{index}. {repo.FullName}");
                index++;
            }

            // Select repository
            Console.WriteLine("\nEnter the index of the repository to backup:");
            int selectedIndex = int.Parse(Console.ReadLine()) - 1; // Assuming user input is 1-indexed
            var selectedRepo = repositories[selectedIndex];
            
            Console.WriteLine(selectedRepo.Name);

            // Download issues for selected repository
            var issues = await github.Issue.GetAllForRepository(selectedRepo.Owner.Login, selectedRepo.Name);

            // Encrypt and save issues to file            // Encrypt and save issues to file
            string encryptedFileName = $"{selectedRepo.Name}_{DateTime.Now:yyyy-MM-dd}.txt";

            // Concatenate all issues into a single string
            StringBuilder stringBuilder = new StringBuilder();
            int num =1;
            if (issues != null && issues.Any()){
            foreach (var issue in issues)
            {
                stringBuilder.AppendLine($"{num}. Title: {issue.Title}");
                stringBuilder.AppendLine($"Body: {issue.Body}");
                stringBuilder.AppendLine("------------");
                num++;
            }}
            else{
                stringBuilder.AppendLine("No issues detected...");
            }
            string allIssuesText = stringBuilder.ToString();

            // Generate random key and IV
            byte[] key = Encryption.GenerateRandomKey(256); // 256-bit key size
            byte[] iv = Encryption.GenerateRandomIV();

            // Encrypt the issues text
            byte[] encryptedData = Encryption.Encrypt(allIssuesText, key, iv);

            // Write encrypted data to file
            File.WriteAllBytes(encryptedFileName, encryptedData);
            InsertIntoDatabase(encryptedFileName, DateTime.Now);

            Console.WriteLine($"Issues encrypted and saved to {encryptedFileName}");

            //ask the user if wants to decrypt
            AskForDecryption(encryptedFileName, key, iv);
           
           //
        }
            public static void InsertIntoDatabase(string fileName, DateTime date)
        {
            using (var connection = new SQLiteConnection("Data Source= source.db"))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS EncryptedFiles (FileName TEXT, Date TEXT)";
                    command.ExecuteNonQuery();
                    command.CommandText = $"INSERT INTO EncryptedFiles (FileName, Date) VALUES ('{fileName}', '{date:yyyy-MM-dd HH:mm:ss}')";
                    command.ExecuteNonQuery();
                }
            }
        }
        public static void AskForDecryption(string encryptedFileName, byte[] key, byte[] iv)
        {
            Console.WriteLine("Do you want to decrypt the file? (Y/N)");
            string response = Console.ReadLine().ToUpper();

            if (response.Equals("Y") || response.Equals("YES"))
            {
                // Read encrypted data from file
                byte[] encryptedData = File.ReadAllBytes(encryptedFileName);

                // Decrypt the data
                string decryptedText = Encryption.Decrypt(encryptedData, key, iv);

                // Display decrypted content
                Console.WriteLine("\nDecrypted Issues:");
                Console.WriteLine(decryptedText);
            }
            else
            {
                Console.WriteLine("Exiting...");
            }
         }


    }

    public class Encryption
    {
        public static byte[] GenerateRandomKey(int keySize)
        {
            using (Aes aes = Aes.Create())
            {
                aes.KeySize = keySize;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        public static byte[] GenerateRandomIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();
                return aes.IV;
            }
        }

        public static byte[] Encrypt(string plainText, byte[] key, byte[] iv)
        {
            byte[] cipheredtext;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        cipheredtext = memoryStream.ToArray();
                    }
                }
            }
            return cipheredtext;
        }

        public static string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)

        {
            string plainText = String.Empty;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
                using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            plainText = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            return plainText;
        }


    }

}
