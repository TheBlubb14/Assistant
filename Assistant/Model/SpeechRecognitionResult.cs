using System;

namespace Assistant.Model
{
    public sealed partial class MyAssistant
    {
        public sealed class SpeechRecognitionResult : EventArgs
        {
            public float Stability { get; set; }

            public string Transcript { get; set; }

            public static explicit operator SpeechRecognitionResult(Google.Assistant.Embedded.V1Alpha2.SpeechRecognitionResult result)
                => new SpeechRecognitionResult()
                {
                    Stability = result.Stability,
                    Transcript = result.Transcript
                };

            public override string ToString()
            {
                return $"Stablility: {Stability} Transcript: {Transcript}";
            }
        }
    }
}
