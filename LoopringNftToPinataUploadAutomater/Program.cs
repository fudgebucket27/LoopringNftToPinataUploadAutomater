using CsvHelper;
using LoopringNftToPinataUploadAutomater;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

string apiKey = Environment.GetEnvironmentVariable("PINATAAPIKEY", EnvironmentVariableTarget.Machine);//you can either set an environmental variable or input it here directly.
string apiKeySecret = Environment.GetEnvironmentVariable("PINATAAPIKEYSECRET", EnvironmentVariableTarget.Machine); //you can either set an environmental variable or input it here directly.
string nftImageDirectoryFilePath = "D:\\frankenloops hd\\pinata images"; //this should point to a directory that only contains your images in the naming format: 1.jpg, 2.jpg, 3.jpg and etc

FileInfo[] nftImageDirectoryFileNames = Directory.GetFiles(nftImageDirectoryFilePath).Select(fn => new FileInfo(fn)).ToArray();
IPinataService pinataService = new PinataService();
List<NftCidTriplet> metadataCIDTriplets = new List<NftCidTriplet>();

foreach (FileInfo nftImageFileInfo in nftImageDirectoryFileNames)
{
    string nftId = nftImageFileInfo.Name.Split('.')[0]; //the source file directory has the nfts named as follows: 1.jpg, 2.jpg, 3.jpg, 4.jpg and etc, split on the '.' to just grab the id portion
    string nftName = $"FrankenLoops HD #{nftId}"; //change this to the name of your nft
    string nftDescription = "FrankenLoops hand drawn with inspiration from images generated through a machine learning algorithm. View the images that inspired the collection at https://fudgey.io/frankenloopshd"; //change this to the description of your nft
    int nftRoyaltyPercantage = 5; //royalty percantage between 0 - 10

    //Submit image to pinata section
    PinataMetadata imageMetadata
        = new PinataMetadata
    {
        name = nftName + " - image",
        keyvalues = new KeyValues
        {
            nameKey = nftName + " - image"
        }
    };
    string pinataImageMetadataJsonString = JsonConvert.SerializeObject(imageMetadata);
    Console.WriteLine($"Uploading {nftName} image to Pinata");
    PinataResponseData? pinataImageResponseData = await pinataService.SubmitPin(apiKey, apiKeySecret, File.ReadAllBytes(nftImageFileInfo.FullName), nftName, pinataMetadata: pinataImageMetadataJsonString);
    Console.WriteLine($"{nftName} image uploaded to Pinata successfully");

    //Submit metadata.json to pinata section
    NftMetadata nftMetadata = new NftMetadata
    {
        name = nftName,
        description = nftDescription,
        image = "ipfs://" + pinataImageResponseData.IpfsHash,
        animation_url = "ipfs://" + pinataImageResponseData.IpfsHash,
        royalty_percentage = nftRoyaltyPercantage,
        collection_metadata = "https://nftinfos.loopring.io/0x2549e16c0d6fd0e6a8e2441e6cfad110d5d78cb7"
    };
    PinataMetadata metadatajson = new PinataMetadata
    {
        name = nftName + " - metadata.json",
        keyvalues = new KeyValues
        {
            nameKey = nftName + " - metadata.json"
        }
    };
    string pinataMetadataJsonString = JsonConvert.SerializeObject(metadatajson);
    string nftMetadataJsonString = JsonConvert.SerializeObject(nftMetadata);
    byte[] nftMetaDataByteArray = Encoding.ASCII.GetBytes(nftMetadataJsonString);
    Console.WriteLine($"Uploading {nftName} metadata to Pinata");
    PinataResponseData? pinataMetadataResponseData = await pinataService.SubmitPin(apiKey, apiKeySecret, nftMetaDataByteArray, "metadata.json", pinataMetadata: pinataMetadataJsonString);
    Console.WriteLine($"{nftName} metadata uploaded to Pinata successfully");
    Console.WriteLine($"Generated CID {pinataMetadataResponseData.IpfsHash}");

    //Add nft cid triple to list for later csv generation
    NftCidTriplet nftCidTriplet = new NftCidTriplet
    {
        Id = nftName,
        MetadataCid = pinataMetadataResponseData.IpfsHash,
        ImageCid = pinataImageResponseData.IpfsHash
    };
    metadataCIDTriplets.Add(nftCidTriplet);
}

//Generate nft cid triplet csv here
if(metadataCIDTriplets.Count > 0)
{
    string csvName = $"{DateTime.Now.ToString("yyyy-mm-dd hh-mm-ss")}.csv";
    using (var writer = new StreamWriter(csvName))
    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
    {
        csv.WriteRecords(metadataCIDTriplets);
        Console.WriteLine($"Generated NFT ID/Metadata CID/Image CID Triplets CSV.");
        Console.WriteLine($"CSV can be found in the following location: {AppDomain.CurrentDomain.BaseDirectory + csvName}");
    }
}
else
{
    Console.WriteLine("Did not generate any Metadata CIDS");
}

Console.WriteLine("Enter any key to end:");
Console.ReadKey();