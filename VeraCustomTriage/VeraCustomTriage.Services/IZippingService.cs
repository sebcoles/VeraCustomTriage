using Ionic.Zip;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using VeraCustomTriage.Services.Configuration;

namespace VeraCustomTriage.Services
{
    public interface IZippingService
    {
        MemoryStream Zip(string filename, List<MemoryStream> datas);
    }

    public class ZippingService : IZippingService
    {
        private ZipConfiguration _zipConfig;

        public ZippingService(IOptions<ZipConfiguration> config)
        {
            _zipConfig = config.Value;
        }
        public MemoryStream Zip(string filename, List<MemoryStream> datas)
        {
            var ms = new MemoryStream();
            ZipFile zip = new ZipFile
            {
                Encryption = EncryptionAlgorithm.WinZipAes256,
                Password = _zipConfig.Password
            };

            foreach (var data in datas)
            {
                data.Seek(0, SeekOrigin.Begin);
                ZipEntry e1 = zip.AddEntry(filename, data);
                e1.Encryption = EncryptionAlgorithm.WinZipAes256;
                e1.Password = _zipConfig.Password;
            }
            ms.Seek(0, SeekOrigin.Begin);
            zip.Save(ms);
            return ms;
        }
    }
}
