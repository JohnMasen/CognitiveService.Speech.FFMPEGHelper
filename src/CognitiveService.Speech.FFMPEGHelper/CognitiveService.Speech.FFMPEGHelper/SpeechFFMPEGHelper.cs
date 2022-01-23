using FFMpegCore;
using FFMpegCore.Pipes;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace CognitiveService.Speech.FFMPEGHelper
{
    public class SpeechFFMPEGHelper
    {
        public StreamCallBackHelper CreateFFMPEGLoader(string filepath,string ffmpegPath,int? pan, string panOutputDefinitions)
        {
            FFOptions options = new FFOptions();
            if (ffmpegPath != null)
            {
                options.BinaryFolder = ffmpegPath;
            }
            //panOutputDefinitions = "c0=0.5*c0+0.5*c1";
            var p = FFMpegArguments.FromFileInput(filepath);
            return (CreateFFMPEGLoader(p,opt=>
            {
                if (pan.HasValue)
                {
                    opt.WithAudioFilters(af =>
                    {
                        af.Pan(pan.Value, panOutputDefinitions);
                    });
                }
            },
            options));
        }
        public StreamCallBackHelper CreateFFMPEGLoader(FFMpegArguments args,Action<FFMpegArgumentOptions> outputCreationCallBack,FFOptions options)
        {
            AnonymousPipeServerStream serverStream = new AnonymousPipeServerStream(PipeDirection.Out);

            StreamPipeSink sink = new StreamPipeSink(serverStream);
            var p = args.OutputToPipe(sink, opt =>
               {
                   opt.WithAudioCodec("pcm_s16le")
                        .WithAudioSamplingRate(16000)
                        .ForceFormat("s16le");
                   outputCreationCallBack?.Invoke(opt);
               });
            Task.Run(() =>
            {
                p.ProcessSynchronously(true, options);
                serverStream.Close();
            });
            return new StreamCallBackHelper(new AnonymousPipeClientStream(PipeDirection.In, serverStream.GetClientHandleAsString()));
        }
    }
}
