using LoopringNftToPinataUploadAutomater;
using Newtonsoft.Json;

for(int i= 889; i <= 1111; i++)
{
    string nftId = i; //the source file directory has the nfts named as follows: 1.jpg, 2.jpg, 3.jpg, 4.jpg and etc, split on the '.' to just grab the id portion
    string nftName = $"FrankenLoop #{nftId}"; //change this to the name of your nft
    string nftDescription = $"It is a mistake to fancy that horror is associated inextricably with darkness, silence, and solitude."; //change this to the description of your nft
    int nftRoyaltyPercantage = 6; //royalty percantage between 0 - 10

    //Submit metadata.json to pinata section
    NftMetadata nftMetadata = new NftMetadata
    {
        name = nftName,
        description = nftDescription,
        image = $"ipfs://QmQsoWPvJV43rBarrAA9DSxeyq5saPnoRxT95s5a4Ard8W/{nftId}.jpg",
        royalty_percentage = nftRoyaltyPercantage
    };
    string nftMetadataJsonString = JsonConvert.SerializeObject(nftMetadata);
    File.WriteAllText($"C:\\temp\\frankenloops5\\{nftId}.json", nftMetadataJsonString);
}

/*
foreach (FileInfo nftImageFileInfo in nftImageDirectoryFileNames)
{


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
        royalty_percentage = nftRoyaltyPercantage
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
*/

Console.WriteLine("Enter any key to end:");
Console.ReadKey();