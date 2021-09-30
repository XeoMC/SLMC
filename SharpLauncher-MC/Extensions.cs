using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLauncher_MC
{
    public static class Extensions
    {
        public static string GetHashSHA1(this byte[] data) // https://stackoverflow.com/questions/800463/c-sharp-create-a-hash-for-a-byte-array-or-image
        {
            using (var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                return string.Concat(sha1.ComputeHash(data).Select(x => x.ToString("X2")));
            }
        }
    }
}
