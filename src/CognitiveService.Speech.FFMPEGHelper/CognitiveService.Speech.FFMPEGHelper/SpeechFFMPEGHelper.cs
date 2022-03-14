using FFMpegCore;
using FFMpegCore.Pipes;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace CognitiveService.Speech.FFMPEGHelper
{
    public class SpeechFFMPEGHelper
    {
        private const string AUDIO_CODEC = "pcm_s16le";
        private const string AUDIO_FORMAT = "s16le";
        private const int SAMPLE_RATE = 16000;
        public static PullAudioInputStream CreateFFMPEGLoader(string filepath,string ffmpegPath,int? pan, string panOutputDefinitions)
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
        public static PullAudioInputStream CreateFFMPEGLoader(FFMpegArguments args,Action<FFMpegArgumentOptions> outputCreationCallBack,FFOptions options)
        {
            var helper = new StreamPipesHelper();
            
            Task.Run(() =>
            {
                var p = args.OutputToPipe(helper.InputStream, opt =>
                {
                    opt.WithAudioCodec(AUDIO_CODEC)
                         .WithAudioSamplingRate(SAMPLE_RATE)
                         .ForceFormat(AUDIO_FORMAT);
                    outputCreationCallBack?.Invoke(opt);
                });
                p.ProcessSynchronously(true, options);
                helper.CloseInputStream();
            });
            return helper.OutputStream;
        }
    }
}
