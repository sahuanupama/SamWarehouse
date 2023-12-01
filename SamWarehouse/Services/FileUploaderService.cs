using System.Security.Cryptography;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Packaging.Signing;

namespace SamWarehouse.Services
    {
    public class FileUploaderService
        {
        private readonly string _uploadPath;
        private readonly EncryptionService _encryptionService;

        public FileUploaderService(IWebHostEnvironment env, EncryptionService encyption)
            {

            _uploadPath = Path.Combine(env.WebRootPath, "Uploads");
            _encryptionService = encyption;
            }


        public void SaveFile(IFormFile file)
            {
            //Create variable to hold our file name
            string fileName = file.FileName;
            //Create variable to hold our file contents once converted to byte array.
            byte[] fileContents;
            //CReate a memory stream to convert the file contents to a byte array.
            using (MemoryStream memStream = new MemoryStream())
                {
                //Pass the file into the memory stream
                file.CopyTo(memStream);
                //Get the file contents back out of the stream as a byte array.
                fileContents = memStream.ToArray();
                }

            //Pass the file contents to the encryption service to be encrypted and get back the encrypted data.
            byte[] encryptedData = _encryptionService.EncryptData(fileContents);

            //Create a memory array to mamnage the encrypted data for file writing.
            using (MemoryStream memStream = new MemoryStream(encryptedData))
                {
                //Combine the filepath and file name to get the complete file path
                string fullFilePath = Path.Combine(_uploadPath, fileName);
                //Craete a file stream to store the encrypted file in our directory back in the files format
                using (FileStream fileStream = new FileStream(fullFilePath, FileMode.Create))
                    {
                    //Pass the byte array of file data through from the moemory stream and through the file
                    //stream to the specified location. 
                    memStream.WriteTo(fileStream);
                    }
                }
            }

        private FileInfo LoadFile(string fileName)
            {
            DirectoryInfo directory = new DirectoryInfo(_uploadPath);
            if (directory.EnumerateFiles().Any(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)))
                {
                return directory.EnumerateFiles().Where(f => f.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                }
            return null;
            }



        private byte[] ReadFileIntoMemory(string fileName)
            {
            var file = LoadFile(fileName);
            if (file == null)
                {
                return null;
                }

            using (var memStream = new MemoryStream())
                {
                using (var fileStream = File.OpenRead(file.FullName))
                    {
                    fileStream.CopyTo(memStream);
                    return memStream.ToArray();
                    }
                }
            }

        public byte[] DownloadFile(string fileName)
            {
            var encryptedFile = ReadFileIntoMemory(fileName);
            if (encryptedFile == null || encryptedFile.Length == 0)
                {
                return null;
                }
            var decrypedData = _encryptionService.DecryptData(encryptedFile);
            return decrypedData;
            }


        }
    }
