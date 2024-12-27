using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
namespace MonitPro.Common.Library
{
 
   public static class Security
    {

        static readonly string PasswordHash = "M@N#T$R%";
        static readonly string SaltKey = "@M@O@N!I!";
        static readonly string VIKey = "M~O!N@I#T$P%R^O&";
         
       public static string Encrypt(string plainText)
       {
           byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

           byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
           var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
           var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

           byte[] cipherTextBytes;

           using (var memoryStream = new MemoryStream())
           {
               using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
               {
                   cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                   cryptoStream.FlushFinalBlock();
                   cipherTextBytes = memoryStream.ToArray();
                   cryptoStream.Close();
               }
               memoryStream.Close();
           }
           return Convert.ToBase64String(cipherTextBytes);
       }


       public static string Decrypt(string encryptedText)
       {
           byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
           byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
           var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

           var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
           var memoryStream = new MemoryStream(cipherTextBytes);
           var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
           byte[] plainTextBytes = new byte[cipherTextBytes.Length];

           int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
           memoryStream.Close();
           cryptoStream.Close();
           return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
       }


       public static bool IsLicenseValid()
       {
           string LicenseKey=ConfigurationManager.AppSettings["LicenseKey"];
          
            for (int i = 0; i < 3; i++)
            {
                LicenseKey = Security.Decrypt(LicenseKey);
            }

           XmlDocument xmlDoc = new XmlDocument();
           xmlDoc.LoadXml(LicenseKey);
           string expiryDate=xmlDoc.SelectSingleNode("M/ED").InnerText;
           DateTime ExpiryDate = DateTime.Parse(expiryDate, new CultureInfo("nl-NL"));

           if (DateTime.Now > ExpiryDate)
               return false;

               return true;
       }

       public static int LicensedEquipmentCount()
       {
           string LicenseKey = ConfigurationManager.AppSettings["LicenseKey"];

           for (int i = 0; i < 3; i++)
           {
               LicenseKey = Security.Decrypt(LicenseKey);
           }

           XmlDocument xmlDoc = new XmlDocument();
           xmlDoc.LoadXml(LicenseKey);
           return int.Parse( xmlDoc.SelectSingleNode("M/EC").InnerText);
           
       }

    }
}