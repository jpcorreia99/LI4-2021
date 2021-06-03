using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PenedaVesGenerator
{
    class Program
    {
        private static HttpClient _client;
        private static Dictionary<int,Species> SpeciesDict{ get; set; }
        private static Dictionary<int,Camera> CamerasDict{ get; set; }


        private static async Task Main()
        {
            var clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            // Pass the handler to httpclient(from you are calling api)
            _client = new HttpClient(clientHandler);
            
            SpeciesDict = await GetSpecies();

            foreach (var species in SpeciesDict.Values)
            {
                Console.WriteLine(species.ToString());
            }
            
            CamerasDict = await GetCameras();

            foreach (var camera in CamerasDict.Values)
            {
                Console.WriteLine(camera.ToString());
            }
            
            SendSighting();
        }

        private static async Task<Dictionary<int,Species>> GetSpecies()
        {
            _client.DefaultRequestHeaders.Accept.Clear();

            var streamTask = _client.GetStreamAsync("https://localhost:5001/api/GetSpecies");
            var speciesList = await JsonSerializer.DeserializeAsync<List<Species>>(await streamTask);
                
            return speciesList
                    .ToDictionary(
                    species =>species.Id,
                    species => species);
        }
        
        private static async Task<Dictionary<int,Camera>> GetCameras()
        {
            _client.DefaultRequestHeaders.Accept.Clear();

            var streamTask = _client.GetStreamAsync("https://localhost:5001/api/GetCameras");
            var camerasList = await JsonSerializer.DeserializeAsync<List<Camera>>(await streamTask);
            
            return camerasList
                  .ToDictionary(
                    camera =>camera.Id,
                    camera => camera);
        }

        private static void SendSighting()
        {
            int speciesId, cameraId, quantity;
            
            Console.WriteLine("0 - choose Id's, 1 - random id's");
            string choice = Console.ReadLine();

            if (choice is "1")
            {
                Random r = new Random();
                List<int> speciesIds =  new List<int>(SpeciesDict.Keys);
                int speciesIndex = r.Next(0, speciesIds.Count);
                speciesId = speciesIds[speciesIndex];
                
                List<int> camerasIds =  new List<int>(CamerasDict.Keys);
                int cameraIndex = r.Next(0, camerasIds.Count);
                cameraId = camerasIds[cameraIndex];
                
                quantity = r.Next(0, 6);
            }
            else
            {
                Console.Write("Species Id:");
                speciesId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Camera Id: ");
                cameraId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Quantity: ");
                quantity = int.Parse(Console.ReadLine() ?? "1");
            }

            string speciesName = SpeciesDict[speciesId].CommonName;
            string cameraName = CamerasDict[cameraId].Name;
            
            var requestSummary = "{\"Camera\":"+ cameraName +",\n" +
                       "\"Species\":"+ speciesName +",\n" +
                       "\"Quantity\":"+ quantity +"}";
            
            Console.WriteLine(requestSummary);
            
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://localhost:5001/api/PostSighting");
            httpWebRequest.ServerCertificateValidationCallback += (_, _, _, _) => true;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            
            var json = "{\"CameraId\":"+ cameraId +"," +
                                "\"SpeciesId\":"+ speciesId +"," +
                                "\"Quantity\":"+ quantity +"}";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            try
            {
                httpWebRequest.GetResponse();
                Console.WriteLine("Success");
            }
            catch (WebException)
            {
                Console.WriteLine("Request failed");
            }
        }
    }
}