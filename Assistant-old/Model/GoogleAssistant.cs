using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assistant.Model
{
    public class GoogleAssistant
    {
        public event EventHandler<GoogleAssistantStatusEventArgs> StatusChanged;

        private readonly AudioManager audioManager;

        public GoogleAssistant()
        {
            audioManager = new AudioManager();
        }

        public void Write(string message)
        {

        }

        public void StartListen()
        {
            audioManager.StartRecording();
            audioManager.PlayInternal();

        }

        public void StopListen()
        {
            audioManager.StopRecording();
            audioManager.StopInternal();
        }
    }
}
