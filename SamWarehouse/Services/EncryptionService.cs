using System.Security.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;


namespace SamWarehouse.Services
    {
    public class EncryptionService
        {
        private readonly string _secretKey;
        public EncryptionService(IConfiguration config)
            {
            _secretKey = config["SecretKey"];
            }

        public byte[] EncryptData(byte[] fileData)
            {
            //Creates our AES algorithm classs(which will automatically generate an Initialization Vector)
            using (Aes aesAlg = Aes.Create())
                {
                //Converts our secret key to a Byte array in UTF-8 encoding.
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);
                //Create a new decryptor object which will use the algorithm with the key and
                //Initialisation Vector(IV) to encrypt our data.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream memStream = new MemoryStream())
                    {
                    //Add the IV to the start of the byte array in the memory stream
                    memStream.Write(aesAlg.IV, 0, 16);

                    using (CryptoStream cryStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                        {
                        cryStream.Write(fileData, 0, fileData.Length);
                        cryStream.FlushFinalBlock();
                        return memStream.ToArray();
                        }
                    }
                }
            }

        public byte[] DecryptData(byte[] encryptedData)
            {
            //Creates our AES algorithm class. This will automatically generate an Initialization Vector, but we will
            //overwrite it wiht the IV from when the file was encrypted.
            using (Aes aesAlg = Aes.Create())
                {
                //Converts our secret key to a Byte array in UTF-8 encoding.
                aesAlg.Key = System.Text.Encoding.UTF8.GetBytes(_secretKey);
                //Create a byte array to hold our Initialisation Vector once we extract it from the file.
                byte[] IV = new byte[16];
                //Copy the first 16 bytes form the file, this is where we put the IV when we encrypted the file.
                Array.Copy(encryptedData, IV, IV.Length);
                //Create an decryptor object to decrypt our file data using the secret key and IV
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, IV);
                //Create a memorystream which will manage and hold our byte array during the process
                using (MemoryStream memStream = new MemoryStream())
                    {
                    //Create a cryptostream which will perfom the decryption using the decryptor object and will push the
                    //final data to the memory stream.
                    using (CryptoStream cryStream = new CryptoStream(memStream, decryptor, CryptoStreamMode.Write))
                        {
                        //Pass the data from the byte array starting at the index after the initilisation vector.
                        cryStream.Write(encryptedData, 16, encryptedData.Length - 16);
                        //Push all the content through the stream until it is completed.
                        cryStream.FlushFinalBlock();
                        //Return the decrypted byte array from the memory stream.
                        return memStream.ToArray();
                        }
                    }
                }
            }
        }
    }
