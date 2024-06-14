using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace GitBackupApp
{
    interface IEncryption
    {

        byte[] GenerateRandomKey(int keySize);

        byte[] GenerateRandomIV();

        byte[] Encrypt(string plainText, byte[] key, byte[] iv);

        string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv);

    }
}
