using Ionic.Zip;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using VeraCustomTriage.Shared.Configuration;

namespace VeraCustomTriage.Shared
{
    public interface IZippingService
    {
        MemoryStream Zip(List<KeyValuePair<string, MemoryStream>> datas, string password);
    }

    public class ZippingService : IZippingService
    {
        public MemoryStream Zip(List<KeyValuePair<string, MemoryStream>> datas, string password)
        {
            var ms = new MemoryStream();
            ZipFile zip = new ZipFile
            {
                Encryption = EncryptionAlgorithm.WinZipAes256,
                Password = password
            };

            foreach (var data in datas)
            {
                data.Value.Seek(0, SeekOrigin.Begin);
                ZipEntry e1 = zip.AddEntry(data.Key, data.Value);
                e1.Encryption = EncryptionAlgorithm.WinZipAes256;
                e1.Password = password;
            }
            ms.Seek(0, SeekOrigin.Begin);
            zip.Save(ms);
            return ms;
        }
    }
}
