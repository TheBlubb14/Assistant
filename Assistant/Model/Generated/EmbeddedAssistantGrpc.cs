// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: google/assistant/embedded/v1alpha2/embedded_assistant.proto
// </auto-generated>
// Original file comments:
// Copyright 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#pragma warning disable 1591
#region Designer generated code

using grpc = global::Grpc.Core;

namespace Google.Assistant.Embedded.V1Alpha2 {
  /// <summary>
  /// Service that implements the Google Assistant API.
  /// </summary>
  public static partial class EmbeddedAssistant
  {
    static readonly string __ServiceName = "google.assistant.embedded.v1alpha2.EmbeddedAssistant";

    static readonly grpc::Marshaller<global::Google.Assistant.Embedded.V1Alpha2.AssistRequest> __Marshaller_AssistRequest = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Google.Assistant.Embedded.V1Alpha2.AssistRequest.Parser.ParseFrom);
    static readonly grpc::Marshaller<global::Google.Assistant.Embedded.V1Alpha2.AssistResponse> __Marshaller_AssistResponse = grpc::Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::Google.Assistant.Embedded.V1Alpha2.AssistResponse.Parser.ParseFrom);

    static readonly grpc::Method<global::Google.Assistant.Embedded.V1Alpha2.AssistRequest, global::Google.Assistant.Embedded.V1Alpha2.AssistResponse> __Method_Assist = new grpc::Method<global::Google.Assistant.Embedded.V1Alpha2.AssistRequest, global::Google.Assistant.Embedded.V1Alpha2.AssistResponse>(
        grpc::MethodType.DuplexStreaming,
        __ServiceName,
        "Assist",
        __Marshaller_AssistRequest,
        __Marshaller_AssistResponse);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::Google.Assistant.Embedded.V1Alpha2.EmbeddedAssistantReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of EmbeddedAssistant</summary>
    public abstract partial class EmbeddedAssistantBase
    {
      /// <summary>
      /// Initiates or continues a conversation with the embedded Assistant Service.
      /// Each call performs one round-trip, sending an audio request to the service
      /// and receiving the audio response. Uses bidirectional streaming to receive
      /// results, such as the `END_OF_UTTERANCE` event, while sending audio.
      ///
      /// A conversation is one or more gRPC connections, each consisting of several
      /// streamed requests and responses.
      /// For example, the user says *Add to my shopping list* and the Assistant
      /// responds *What do you want to add?*. The sequence of streamed requests and
      /// responses in the first gRPC message could be:
      ///
      /// *   AssistRequest.config
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistResponse.event_type.END_OF_UTTERANCE
      /// *   AssistResponse.speech_results.transcript "add to my shopping list"
      /// *   AssistResponse.dialog_state_out.microphone_mode.DIALOG_FOLLOW_ON
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      ///
      /// The user then says *bagels* and the Assistant responds
      /// *OK, I've added bagels to your shopping list*. This is sent as another gRPC
      /// connection call to the `Assist` method, again with streamed requests and
      /// responses, such as:
      ///
      /// *   AssistRequest.config
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistResponse.event_type.END_OF_UTTERANCE
      /// *   AssistResponse.dialog_state_out.microphone_mode.CLOSE_MICROPHONE
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      ///
      /// Although the precise order of responses is not guaranteed, sequential
      /// `AssistResponse.audio_out` messages will always contain sequential portions
      /// of audio.
      /// </summary>
      /// <param name="requestStream">Used for reading requests from the client.</param>
      /// <param name="responseStream">Used for sending responses back to the client.</param>
      /// <param name="context">The context of the server-side call handler being invoked.</param>
      /// <returns>A task indicating completion of the handler.</returns>
      public virtual global::System.Threading.Tasks.Task Assist(grpc::IAsyncStreamReader<global::Google.Assistant.Embedded.V1Alpha2.AssistRequest> requestStream, grpc::IServerStreamWriter<global::Google.Assistant.Embedded.V1Alpha2.AssistResponse> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Client for EmbeddedAssistant</summary>
    public partial class EmbeddedAssistantClient : grpc::ClientBase<EmbeddedAssistantClient>
    {
      /// <summary>Creates a new client for EmbeddedAssistant</summary>
      /// <param name="channel">The channel to use to make remote calls.</param>
      public EmbeddedAssistantClient(grpc::Channel channel) : base(channel)
      {
      }
      /// <summary>Creates a new client for EmbeddedAssistant that uses a custom <c>CallInvoker</c>.</summary>
      /// <param name="callInvoker">The callInvoker to use to make remote calls.</param>
      public EmbeddedAssistantClient(grpc::CallInvoker callInvoker) : base(callInvoker)
      {
      }
      /// <summary>Protected parameterless constructor to allow creation of test doubles.</summary>
      protected EmbeddedAssistantClient() : base()
      {
      }
      /// <summary>Protected constructor to allow creation of configured clients.</summary>
      /// <param name="configuration">The client configuration.</param>
      protected EmbeddedAssistantClient(ClientBaseConfiguration configuration) : base(configuration)
      {
      }

      /// <summary>
      /// Initiates or continues a conversation with the embedded Assistant Service.
      /// Each call performs one round-trip, sending an audio request to the service
      /// and receiving the audio response. Uses bidirectional streaming to receive
      /// results, such as the `END_OF_UTTERANCE` event, while sending audio.
      ///
      /// A conversation is one or more gRPC connections, each consisting of several
      /// streamed requests and responses.
      /// For example, the user says *Add to my shopping list* and the Assistant
      /// responds *What do you want to add?*. The sequence of streamed requests and
      /// responses in the first gRPC message could be:
      ///
      /// *   AssistRequest.config
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistResponse.event_type.END_OF_UTTERANCE
      /// *   AssistResponse.speech_results.transcript "add to my shopping list"
      /// *   AssistResponse.dialog_state_out.microphone_mode.DIALOG_FOLLOW_ON
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      ///
      /// The user then says *bagels* and the Assistant responds
      /// *OK, I've added bagels to your shopping list*. This is sent as another gRPC
      /// connection call to the `Assist` method, again with streamed requests and
      /// responses, such as:
      ///
      /// *   AssistRequest.config
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistResponse.event_type.END_OF_UTTERANCE
      /// *   AssistResponse.dialog_state_out.microphone_mode.CLOSE_MICROPHONE
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      ///
      /// Although the precise order of responses is not guaranteed, sequential
      /// `AssistResponse.audio_out` messages will always contain sequential portions
      /// of audio.
      /// </summary>
      /// <param name="headers">The initial metadata to send with the call. This parameter is optional.</param>
      /// <param name="deadline">An optional deadline for the call. The call will be cancelled if deadline is hit.</param>
      /// <param name="cancellationToken">An optional token for canceling the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::Google.Assistant.Embedded.V1Alpha2.AssistRequest, global::Google.Assistant.Embedded.V1Alpha2.AssistResponse> Assist(grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
      {
        return Assist(new grpc::CallOptions(headers, deadline, cancellationToken));
      }
      /// <summary>
      /// Initiates or continues a conversation with the embedded Assistant Service.
      /// Each call performs one round-trip, sending an audio request to the service
      /// and receiving the audio response. Uses bidirectional streaming to receive
      /// results, such as the `END_OF_UTTERANCE` event, while sending audio.
      ///
      /// A conversation is one or more gRPC connections, each consisting of several
      /// streamed requests and responses.
      /// For example, the user says *Add to my shopping list* and the Assistant
      /// responds *What do you want to add?*. The sequence of streamed requests and
      /// responses in the first gRPC message could be:
      ///
      /// *   AssistRequest.config
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistResponse.event_type.END_OF_UTTERANCE
      /// *   AssistResponse.speech_results.transcript "add to my shopping list"
      /// *   AssistResponse.dialog_state_out.microphone_mode.DIALOG_FOLLOW_ON
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      ///
      /// The user then says *bagels* and the Assistant responds
      /// *OK, I've added bagels to your shopping list*. This is sent as another gRPC
      /// connection call to the `Assist` method, again with streamed requests and
      /// responses, such as:
      ///
      /// *   AssistRequest.config
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistRequest.audio_in
      /// *   AssistResponse.event_type.END_OF_UTTERANCE
      /// *   AssistResponse.dialog_state_out.microphone_mode.CLOSE_MICROPHONE
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      /// *   AssistResponse.audio_out
      ///
      /// Although the precise order of responses is not guaranteed, sequential
      /// `AssistResponse.audio_out` messages will always contain sequential portions
      /// of audio.
      /// </summary>
      /// <param name="options">The options for the call.</param>
      /// <returns>The call object.</returns>
      public virtual grpc::AsyncDuplexStreamingCall<global::Google.Assistant.Embedded.V1Alpha2.AssistRequest, global::Google.Assistant.Embedded.V1Alpha2.AssistResponse> Assist(grpc::CallOptions options)
      {
        return CallInvoker.AsyncDuplexStreamingCall(__Method_Assist, null, options);
      }
      /// <summary>Creates a new instance of client from given <c>ClientBaseConfiguration</c>.</summary>
      protected override EmbeddedAssistantClient NewInstance(ClientBaseConfiguration configuration)
      {
        return new EmbeddedAssistantClient(configuration);
      }
    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    public static grpc::ServerServiceDefinition BindService(EmbeddedAssistantBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_Assist, serviceImpl.Assist).Build();
    }

  }
}
#endregion
