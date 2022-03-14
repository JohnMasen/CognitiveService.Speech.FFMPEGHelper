using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CognitiveService.Speech.FFMPEGHelper.Sample
{
    internal class Program
    {
        static StringBuilder sb;
        static async Task Main(string[] args)
        {
            string uri = "https://chinaeast2.api.cognitive.azure.cn/sts/v1.0/issuetoken";
            string key = "927aa070b2d24db3a7d1167c4cfa9a56";
            string ffmpegPath = @"d:\tools\ffmpeg\bin";

            var speechConfig = SpeechConfig.FromEndpoint(new Uri(uri), key);
            // Create an audio config specifying the compressed
            // audio format and the instance of your input stream class.
            //var audioFormat = AudioStreamFormat.GetDefaultInputFormat();
            var s = SpeechFFMPEGHelper.CreateFFMPEGLoader("OSR_us_000_0030_8k.ogg", ffmpegPath, null, null);
            sb = new StringBuilder();
            //var s = new PullAudioInputStream(b, audioFormat);
            var audioConfig = AudioConfig.FromStreamInput(s);
            SourceLanguageConfig languageConfig = SourceLanguageConfig.FromLanguage("EN-US");
            using var recognizer = new SpeechRecognizer(speechConfig, languageConfig, audioConfig);
            ManualResetEvent mre = new ManualResetEvent(false);
            recognizer.Recognized += (o, e) =>
            {
                voiceRecognizedCallback(e.Result);
            };
            recognizer.SpeechEndDetected += (o, e) =>
            {
                Console.WriteLine("Speech Stop");
                Debug.Write(sb.ToString());
                mre.Set();
            };
            recognizer.SpeechStartDetected += (o, e) =>
            {
                Console.WriteLine("Speech Start");
            };
            await recognizer.StartContinuousRecognitionAsync();
            mre.WaitOne();
        }
        static void voiceRecognizedCallback(SpeechRecognitionResult result)
        {
            string s = $"{TimeSpan.FromTicks(result.OffsetInTicks)},{result.Reason},{result.Text}";
            sb.AppendLine(s);
            Console.WriteLine(s);
        }
    }
}
