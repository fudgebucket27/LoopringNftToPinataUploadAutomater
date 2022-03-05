using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopringNftToPinataUploadAutomater
{
    public interface IPinataService
    {
        Task<PinataResponseData?> SubmitPin(string apikey, string apiKeySecret, byte[] fileBytes, string fileName, bool wrapDirectory = false, string pinataMetadata = null);
    }
}
