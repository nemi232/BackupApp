using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Octokit; // Assuming GitHub API is used


namespace GitBackupApp{
    interface IClientCred
    {
        GitHubClient ClientCred();
    }
    public  class ClentCredentials : IClientCred{
        public  GitHubClient ClientCred()
        {
            var github = new GitHubClient(new ProductHeaderValue("May"));
            var tokenAuth = new Credentials("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");
            github.Credentials = tokenAuth;
            return github;
        }

    }


}