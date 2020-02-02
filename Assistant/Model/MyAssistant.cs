using Google.Assistant.Embedded.V1Alpha2;
using Google.Protobuf;
using GoogleLibrary.User;
using Grpc.Core;
using Grpc.Net.Client;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Assistant.Model
{
    public sealed partial class MyAssistant : IDisposable
    {
        /// <summary>
        /// The Assistant want to Display Text
        /// </summary>
        public event EventHandler<string> DisplayText;

        /// <summary>
        /// The Assistant want to Display HTML5
        /// </summary>
        public event EventHandler<string> DisplayHtml;

        /// <summary>
        /// The Assiatant want to let you know what hes understanding
        /// </summary>
        public event EventHandler<SpeechRecognitionResult> SpeechRecognized;

        /// <summary>
        /// The Assistant is waiting for a Follow Up Text
        /// </summary>
        public event EventHandler<FollowUpEventArgs> WaitingForFollowUp;

        private const string USER = "Assistant";
        private const string ENDPOINT = "https://embeddedassistant.googleapis.com";
        private const string DATA_STORE_FOLDER = "Assistant";

        private readonly List<string> scope = new List<string>()
        {
            "openid",
            "https://www.googleapis.com/auth/assistant-sdk-prototype"
        };

        private UserManager userManager;
        private GrpcChannel channel;
        private EmbeddedAssistant.EmbeddedAssistantClient assistant;
        private PersistanceManager persistance;
        private AudioManager audio;
        private AsyncDuplexStreamingCall<AssistRequest, AssistResponse> call;
        private FollowOn followOn;

        public async Task Initialize()
        {
            persistance = new PersistanceManager(DATA_STORE_FOLDER);
            audio = new AudioManager();
            audio.AudioPlaybackChanged += Audio_AudioPlaybackChanged;
            audio.RecordingDataReceived += Audio_RecordingDataReceived;
            audio.RecordingStopped += Audio_RecordingStopped;

            userManager = await UserManager.Initialize(
                UserManager.GetClientSecretsFromFile(@"B:\sync\assistant-secrets.json"),
                USER, ENDPOINT, scope, DATA_STORE_FOLDER);

            channel = GrpcChannel.ForAddress(ENDPOINT, new GrpcChannelOptions { Credentials = userManager.ChannelCredential });
            assistant = new EmbeddedAssistant.EmbeddedAssistantClient(channel);
        }

        private async void Audio_RecordingDataReceived(object sender, byte[] e)
        {
            // stream / send
            var request = new AssistRequest()
            {
                AudioIn = ByteString.CopyFrom(e)
            };

            await call.RequestStream.WriteAsync(request);
        }

        private async void Audio_RecordingStopped(object sender, EventArgs e)
        {
            await call.RequestStream.CompleteAsync();
        }

        private async void Audio_AudioPlaybackChanged(object sender, bool isPlaying)
        {
            if (isPlaying)
            {
                Log.Information("Assistant is speaking");
            }
            else
            {
                switch (followOn)
                {
                    case FollowOn.Nothing:
                        Log.Information("Assistant is in Idle");
                        break;

                    case FollowOn.Audio:
                        Log.Information("Assistant is waiting for next Audio Conversation");
                        await NewAudioConversation(false);
                        break;

                    case FollowOn.Text:
                        Log.Information("Assistant is waiting for next Text Conversation");
                        var followUp = new FollowUpEventArgs();
                        WaitingForFollowUp?.Invoke(this, followUp);
                        await NewTextConversation(followUp.Text, false);
                        break;
                }
            }
        }

        public async Task NewAudioConversation(bool isNewConversation)
        {
            followOn = FollowOn.Audio;
            call = assistant.Assist();
            await call.RequestStream.WriteAsync(CreateAudioRequest(isNewConversation)).ConfigureAwait(false);

            StartStreamingAudio();
            await WaitForResponse();
        }

        public async Task NewTextConversation(string query, bool isNewConversation)
        {
            followOn = FollowOn.Text;
            call = assistant.Assist();
            await call.RequestStream.WriteAsync(CreateTextRequest(query, isNewConversation)).ConfigureAwait(false);

            await WaitForResponse();
        }

        private async Task WaitForResponse()
        {
            await foreach (var response in call.ResponseStream.ReadAllAsync())
            {
                // Assistant has recognised something, Stop sending Audio
                if (response.EventType == AssistResponse.Types.EventType.EndOfUtterance)
                {
                    Log.Information("Assistant has recognised Speech, Stop sending Audio");
                    StopStreamingAudio();
                }

                // Add Audio to Play-Buffer
                if (response.AudioOut != null)
                {
                    Log.Debug("Adding response Audio to buffer");
                    response.AudioOut.AudioData.WriteTo(audio.OutputStream);
                }

                // Log Debug Information
                if (response.DebugInfo != null)
                    Log.Debug(response.DebugInfo.AogAgentToAssistantJson);

                // Current Dialog
                if (response.DialogStateOut != null)
                {
                    persistance.UpdateConversation(response.DialogStateOut.ConversationState);
                    audio.VolumePercentage = response.DialogStateOut.VolumePercentage;

                    switch (response.DialogStateOut.MicrophoneMode)
                    {
                        case DialogStateOut.Types.MicrophoneMode.Unspecified:
                            Log.Information("MicrophoneMode.Unspecified");
                            followOn = FollowOn.Nothing;
                            break;

                        case DialogStateOut.Types.MicrophoneMode.CloseMicrophone:
                            Log.Information("MicrophoneMode.CloseMicrophone -> End of Conversation");
                            followOn = FollowOn.Nothing;
                            break;

                        case DialogStateOut.Types.MicrophoneMode.DialogFollowOn:
                            Log.Information("MicrophoneMode.DialogFollowOn -> End of Conversation");
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(response.DialogStateOut.SupplementalDisplayText))
                    {
                        var text = response.DialogStateOut.SupplementalDisplayText;
                        Log.Information("Assistant displays Text");
                        Log.Information(text);
                        DisplayText?.Invoke(this, text);
                    }
                }

                // HTML5 Visuals
                if (response.ScreenOut?.Data != null && response.ScreenOut?.Data != ByteString.Empty)
                {
                    switch (response.ScreenOut.Format)
                    {
                        case ScreenOut.Types.Format.Html:
                            var html = response.ScreenOut.Data.ToStringUtf8();
                            Log.Information("Assistant displays HTML5");
                            Log.Information(html);
                            DisplayHtml?.Invoke(this, html);
                            break;
                    }
                }

                // Recognized Speech
                if (response.SpeechResults != null)
                {
                    var maxStability = response.SpeechResults.Max(x => x.Stability);
                    var recognition = response.SpeechResults.FirstOrDefault(x => x.Stability == maxStability);

                    if (recognition != null)
                    {
                        Log.Information("Assistant recognized Speech");
                        Log.Information(((SpeechRecognitionResult)recognition).ToString());
                        SpeechRecognized?.Invoke(this, (SpeechRecognitionResult)recognition);
                    }
                }
            }

            Log.Information("End of Response");

            // Play the received Audio
            audio.Play();
        }

        private void StartStreamingAudio()
        {
            audio.StartRecording();
        }

        public void StopStreamingAudio()
        {
            audio.StopRecording();
        }

        #region Create Helpers
        private AssistRequest CreateAudioRequest(bool isNewConversation)
        {
            return new AssistRequest()
            {
                Config = new AssistConfig
                {
                    AudioInConfig = CreateAudioInConfig(),
                    AudioOutConfig = CreateAudioOutConfig(),
                    DebugConfig = CreateDebugConfig(),
                    DialogStateIn = CreateDialogStateIn(isNewConversation),
                    ScreenOutConfig = CreateScreenOutConfig(),
                    // Dont send TextQuery
                }
            };
        }

        private AudioInConfig CreateAudioInConfig()
        {
            return new AudioInConfig()
            {
                Encoding = AudioInConfig.Types.Encoding.Flac,
                SampleRateHertz = AudioManager.SAMPLE_RATE_HZ
            };
        }

        private AssistRequest CreateTextRequest(string query, bool isNewConversation)
        {
            return new AssistRequest()
            {
                Config = new AssistConfig
                {
                    // Dont set AudioInConfig 
                    AudioOutConfig = CreateAudioOutConfig(),
                    DebugConfig = CreateDebugConfig(),
                    DialogStateIn = CreateDialogStateIn(isNewConversation),
                    ScreenOutConfig = CreateScreenOutConfig(),
                    TextQuery = query,
                    DeviceConfig = CreateDeviceConfig()
                }
            };
        }

        private AudioOutConfig CreateAudioOutConfig()
        {
            return new AudioOutConfig()
            {
                Encoding = AudioOutConfig.Types.Encoding.Linear16,
                SampleRateHertz = AudioManager.SAMPLE_RATE_HZ,
                VolumePercentage = audio.VolumePercentage
            };
        }

        private DebugConfig CreateDebugConfig()
        {
            return new DebugConfig()
            {
                ReturnDebugInfo = Debugger.IsAttached
            };
        }

        private DialogStateIn CreateDialogStateIn(bool isNewConversation)
        {
            return new DialogStateIn()
            {
                IsNewConversation = isNewConversation,
                ConversationState = persistance.ReadConversation(),
                LanguageCode = CultureInfo.CurrentCulture.TextInfo.CultureName
            };
        }

        private ScreenOutConfig CreateScreenOutConfig()
        {
            return new ScreenOutConfig()
            {
                ScreenMode = ScreenOutConfig.Types.ScreenMode.Playing
            };
        }

        private DeviceConfig CreateDeviceConfig()
        {
            return new DeviceConfig()
            {
                // missing https://developers.google.com/assistant/sdk/reference/device-registration/model-and-instance-schemas 
            };
        }
        #endregion

        public void Dispose()
        {
            channel?.Dispose();
            channel = null;
        }
    }
}
