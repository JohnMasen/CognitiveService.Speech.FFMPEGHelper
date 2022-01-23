using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CognitiveService.Speech.FFMPEGHelper
{
    public class StreamCallBackHelper : PullAudioInputStreamCallback
    {
        Stream s;
        public StreamCallBackHelper(Stream stream)
        {
            s = stream;
        }
        public override int Read(byte[] dataBuffer, uint size)
        {
            return s.Read(dataBuffer, 0, (int)size);
        }
    }
}
