using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace LoopringNftToPinataUploadAutomater
{
    public class PinataService : IPinataService, IDisposable
    {
        const string _baseUrl = "https://api.pinata.cloud";

        readonly RestClient _client;

        public PinataService()
        {
            _client = new RestClient(_baseUrl);
        }

        public async Task<PinataResponseData?> SubmitPin(string apiKey, string apiKeySecret, byte[] fileBytes, string fileName, bool wrapDirectory = false, string metadataGuid = null)
        {
            var request = new RestRequest("pinning/pinFileToIPFS");
            request.AddHeader("pinata_api_key", apiKey);
            request.AddHeader("pinata_secret_api_key", apiKeySecret);
            request.AddFile("file", fileBytes, fileName);
            if (wrapDirectory == true)
            {
                request.AddParameter("pinataOptions", "{\"wrapWithDirectory\" :true}");
                request.AddParameter("pinataMetadata", metadataGuid);
            }
            try
            {
                var response = await _client.PostAsync(request);
                var data = JsonConvert.DeserializeObject<PinataResponseData>(response.Content!);
                return data;
            }
            catch (HttpRequestException httpException)
            {
                if(wrapDirectory == true)
                {
                    Console.WriteLine($"Issue uploading metadata file for {fileName}, Exception message: {httpException.Message}");
                    return null;
                }
                else
                {
                    Console.WriteLine($"Issue uploading image file for {fileName}, Exception message: {httpException.Message}");
                    return null;
                }
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
