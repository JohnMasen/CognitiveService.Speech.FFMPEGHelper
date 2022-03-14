using FFMpegCore.Pipes;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CognitiveService.Speech.FFMPEGHelper
{
    public class StreamPipesHelper 
    {

        public PullAudioInputStream OutputStream { get; private set; }
        public IPipeSink InputStream { get; private set; }
        AnonymousPipeServerStream server; 
        AnonymousPipeClientStream client; 
        public StreamPipesHelper()
        {
            server = new AnonymousPipeServerStream(PipeDirection.Out);
            client = new AnonymousPipeClientStream(PipeDirection.In, server.ClientSafePipeHandle);
            OutputStream = new PullAudioInputStream(new SpeechServiceStreamCallback(client),AudioStreamFormat.GetDefaultInputFormat());
            InputStream = new StreamPipeSink(server);
        }
        public void CloseInputStream()
        {
            server.Close();
        }
        public class SpeechServiceStreamCallback:PullAudioInputStreamCallback
        {
            Stream s;
            public SpeechServiceStreamCallback(AnonymousPipeClientStream stream)
            {
                s = stream;
            }

            public override int Read(byte[] dataBuffer, uint size)
            {
                return s.Read(dataBuffer, 0, (int)size);
            }
        }

    }
}
