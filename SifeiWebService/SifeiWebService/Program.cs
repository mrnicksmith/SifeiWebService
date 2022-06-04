using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SifeiWebService
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create the folder sifei on the c: drive
            //Create v4 xml and zip in this directory
            

            string fileNameAndPath = @"c:\sifei\v4_testing_CORREGIDO.zip";
            string sifeiUsuario = "CMS2101279B0";
            string sifeiPassword = "b238728e";
            string idEquipo = "MTY1NzY5ZC03YTViLTFjZTAtYTI5Mi02MjlhMWI0YWQxOGU=";



            byte[] sifeiZip = File.ReadAllBytes(fileNameAndPath.ToLower());
            Console.WriteLine("Reading zipfile:" + fileNameAndPath + " into byte array");

            SifeiWebServiceDev.SIFEIClient sIFEIClient = new SifeiWebServiceDev.SIFEIClient();
            Console.WriteLine("Creating webservice client: "+ sIFEIClient.Endpoint.Name);

            byte[] sifev4TimbreArray = null;

            try
            {
                //Call the getCFDI web service with the credentials and zip file
                sifev4TimbreArray = sIFEIClient.getCFDI(sifeiUsuario, sifeiPassword, sifeiZip, "", idEquipo);
            }
            catch (Exception ex)
            {
                //Catch the exception
                Console.WriteLine("There was an exception in the webservice: " + ex.Message);
            }

            XmlDocument sifev4TimbreXml = new XmlDocument();
            MemoryStream ms = new MemoryStream(sifev4TimbreArray);

            //MemoryStream data = new MemoryStream();
            ZipFile zf = null;
            String fullZipToPath = "";
            zf = new ZipFile(ms);
            foreach (ZipEntry zipEntry in zf)
            {
                String entryFileName = zipEntry.Name;
                byte[] buffer = new byte[4096];     // 4K is optimum
                Stream zipStream = zf.GetInputStream(zipEntry);
                fullZipToPath = Path.Combine(@"c:\sifei\", entryFileName);
                string directoryName = Path.GetDirectoryName(fullZipToPath);
                if (directoryName.Length > 0)
                    Directory.CreateDirectory(directoryName);

                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                // of the file, but does not waste memory.
                // The "using" will close the stream even if an exception occurs.
                using (FileStream streamWriter = File.Create(fullZipToPath))
                {
                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                }
            }


        }
    }
}
