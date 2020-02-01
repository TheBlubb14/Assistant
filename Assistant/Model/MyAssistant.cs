using Google.Assistant.Embedded.V1Alpha2;
using GoogleLibrary.User;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assistant.Model
{
    public sealed class MyAssistant : IDisposable
    {
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
        private EmbeddedAssistant.EmbeddedAssistantClient embeddedAssistant;

        public MyAssistant()
        {
        }

        public async Task Initialize()
        {
            userManager = await UserManager.Initialize(
                UserManager.GetClientSecretsFromFile(@"B:\sync\assistant-secrets.json"),
                USER, ENDPOINT, scope, DATA_STORE_FOLDER);

            channel = GrpcChannel.ForAddress(ENDPOINT, new GrpcChannelOptions { Credentials = userManager.ChannelCredential });
            embeddedAssistant = new EmbeddedAssistant.EmbeddedAssistantClient(channel);
        }

        public void Dispose()
        {
            channel?.Dispose();
            channel = null;
        }
    }
}
