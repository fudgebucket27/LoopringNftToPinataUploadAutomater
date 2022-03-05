using CsvHelper;
using LoopringNftToPinataUploadAutomater;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

string apiKey = Environment.GetEnvironmentVariable("PINATAAPIKEY", EnvironmentVariableTarget.Machine);
string apiKeySecret = Environment.GetEnvironmentVariable("PINATAAPIKEYSECRET", EnvironmentVariableTarget.Machine);
string nftImageDirectoryFilePath = "C:\\NFT\\FrankenLoops";

FileInfo[] nftImageDirectoryFileNames = Directory.GetFiles(nftImageDirectoryFilePath).Select(fn => new FileInfo(fn)).ToArray();
IPinataService pinataService = new PinataService();
List<NftCidPair> metadataCIDPairs = new List<NftCidPair>();
foreach (FileInfo nftImageFileInfo in nftImageDirectoryFileNames)
{
    string frankenLoopId = nftImageFileInfo.Name.Split('.')[0];
    string frankenLoopname = $"FrankenLoop #{frankenLoopId}";
    string frankenLoopDescription = "It is a mistake to fancy that horror is associated inextricably with darkness, silence, and solitude.";
    Guid generatedGuidForImage = Guid.NewGuid();
    string imageGuid = generatedGuidForImage.ToString();
    Console.WriteLine($"Uploading {frankenLoopname} image to Pinata");
    PinataResponseData? pinataImageResponseData = await pinataService.SubmitPin(apiKey, apiKeySecret, File.ReadAllBytes(nftImageFileInfo.FullName), frankenLoopname);
    Console.WriteLine($"{frankenLoopname} image uploaded to Pinata successfully");
    NftMetadata nftMetadata = new NftMetadata
    {
        name = frankenLoopname,
        description = frankenLoopDescription,
        image = "ipfs://" + pinataImageResponseData.IpfsHash
    };
    Guid generatedGuidForMetadata = Guid.NewGuid();
    string metadataGuidString = generatedGuidForMetadata.ToString();
    MetadataGuid metadataGuid = new MetadataGuid
    {
        name = metadataGuidString
    };
    string metadataGuidJsonString = JsonConvert.SerializeObject(metadataGuid);
    string metaDataJsonString = JsonConvert.SerializeObject(nftMetadata);
    byte[] metaDataByteArray = Encoding.ASCII.GetBytes(metaDataJsonString);
    Console.WriteLine($"Uploading {frankenLoopname} metadata to Pinata");
    PinataResponseData? pinataMetadataResponseData = await pinataService.SubmitPin(apiKey, apiKeySecret, metaDataByteArray, "metadata.json", true, metadataGuidJsonString);
    Console.WriteLine($"{frankenLoopname} metadata uploaded to Pinata successfully");
    Console.WriteLine($"Generated CID {pinataMetadataResponseData.IpfsHash}");
    NftCidPair nftCidPair = new NftCidPair
    {
        Id = frankenLoopname,
        MetadataCid = pinataMetadataResponseData.IpfsHash
    };
    metadataCIDPairs.Add(nftCidPair);
}

if(metadataCIDPairs.Count > 0)
{
    string csvName = $"{DateTime.Now.ToString("yyyy-mm-dd hh-mm-ss")}.csv";
    using (var writer = new StreamWriter(csvName))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(metadataCIDPairs);
        Console.WriteLine($"Generated NFT ID/Metadata CID Pairs csv: {csvName} in current directory");
    }
}
else
{
    Console.WriteLine("Did not generate any Metadata CIDS");
}


Console.WriteLine("Enter any key to end:");
Console.ReadKey();