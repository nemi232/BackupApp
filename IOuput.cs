using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace GitBackupApp{
     interface IOutput{
        void PrintRepos(GitHubClient github);
         void PrintFiles(StringBuilder stringBuilder, string encryptedFileName);
         void AskForDecryption(string encryptedFileName, byte[] key, byte[] iv);
    }
}