using System;

namespace Assistant.Model
{
    public sealed partial class MyAssistant
    {
        public class FollowUpEventArgs : EventArgs
        {
            public string Text { get; set; }
        }
    }
}
