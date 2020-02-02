using Google.Protobuf;
using System;
using System.IO;

namespace Assistant.Model
{
    public sealed class PersistanceManager
    {
        private readonly DirectoryInfo directoryInfo;
        private readonly FileInfo conversationInfo;

        public PersistanceManager(string folderName)
        {
            directoryInfo = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), folderName));

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            conversationInfo = new FileInfo(Path.Combine(directoryInfo.FullName, "conversation"));
        }

        public void UpdateConversation(ByteString conversation)
        {
            File.WriteAllBytes(conversationInfo.FullName, conversation.ToByteArray());
        }

        public ByteString ReadConversation()
        {
            return conversationInfo.Exists ?
                ByteString.CopyFrom(File.ReadAllBytes(conversationInfo.FullName)) :
                ByteString.Empty;
        }

        public void Delete()
        {
            if (conversationInfo.Exists)
                conversationInfo.Delete();
        }
    }
}
