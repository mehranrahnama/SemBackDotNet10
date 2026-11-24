// C# migration of utils/security/SecurityCipher.java

namespace Common.utils.security;

using System.Security.Cryptography;
using System.Text;
public static class SecurityCipher
{
 public static string EncryptGCM(string plaintext,string text) {
  byte[] salt=RandomNumberGenerator.GetBytes(16),iv=RandomNumberGenerator.GetBytes(12);
  byte[] key=Rfc2898DeriveBytes.Pbkdf2(text,salt,100000,HashAlgorithmName.SHA256,32);
  try {
   byte[] data=Encoding.UTF8.GetBytes(plaintext),encrypted=new byte[data.Length],tag=new byte[16];
   using var aes=new AesGcm(key,16);aes.Encrypt(iv,data,encrypted,tag);
   return Convert.ToBase64String([..encrypted,..tag])+":"+Convert.ToBase64String(iv)+":"+Convert.ToBase64String(salt);
  } finally {CryptographicOperations.ZeroMemory(key);}
 }
 public static string DecryptGCM(string ciphertext,string text) {
  var parts=ciphertext.Split(':');if(parts.Length!=3)throw new FormatException("Expected ciphertext:iv:salt");
  byte[] data=Convert.FromBase64String(parts[0]),iv=Convert.FromBase64String(parts[1]),salt=Convert.FromBase64String(parts[2]);
  if(data.Length<16 || iv.Length!=12 || salt.Length!=16)throw new FormatException("Invalid AES-GCM envelope");
  byte[] key=Rfc2898DeriveBytes.Pbkdf2(text,salt,100000,HashAlgorithmName.SHA256,32);
  try {
   byte[] plain=new byte[data.Length-16];using var aes=new AesGcm(key,16);
   aes.Decrypt(iv,data.AsSpan(0,data.Length-16),data.AsSpan(data.Length-16),plain);
   return Encoding.UTF8.GetString(plain);
  } finally {CryptographicOperations.ZeroMemory(key);}
 }
 [Obsolete("AES-ECB is insecure. Only for compatibility with existing data.")]
 public static string? Encrypt(string? plaintext,string keyVal) {
  if(plaintext is null)return null;
  try { using var aes=Aes.Create();aes.Key=SHA1.HashData(Encoding.UTF8.GetBytes(keyVal))[..16];
   return Convert.ToBase64String(aes.EncryptEcb(Encoding.UTF8.GetBytes(plaintext),PaddingMode.PKCS7));
  } catch(CryptographicException) {return null;}
 }
 [Obsolete("AES-ECB is insecure. Only for compatibility with existing data.")]
 public static string? Decrypt(string? ciphertext,string keyVal) {
  if(ciphertext is null)return null;
  try { using var aes=Aes.Create();aes.Key=SHA1.HashData(Encoding.UTF8.GetBytes(keyVal))[..16];
   return Encoding.UTF8.GetString(aes.DecryptEcb(Convert.FromBase64String(ciphertext),PaddingMode.PKCS7));
  } catch(Exception ex) when(ex is CryptographicException or FormatException) {return null;}
 }
}
