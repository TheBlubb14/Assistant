using Assistant.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace Assistant.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public double MicrophoneOpacity { get; set; }

        public double TextInputOpacity { get; set; }

        public bool Listening { get; set; }

        public bool TextInput { get; set; }

        public string TextInputText { get; set; }

        public string Text { get; set; }

        public ICommand TriggerMicrophoneCommand { get; set; }

        public ICommand TriggerTextInputCommand { get; set; }

        public ICommand TextInputKeyPressCommand { get; set; }

        private GoogleAssistant googleAssistant;

        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                googleAssistant = new GoogleAssistant();
            }

            Text = "whats up?";
            MicrophoneOpacity = 0.3;
            TextInputOpacity = 0.3;
            TextInput = false;
            Listening = false;
            TriggerMicrophoneCommand = new RelayCommand(TriggerMicrophone);
            TriggerTextInputCommand = new RelayCommand(TriggerTextInput);
            TextInputKeyPressCommand = new RelayCommand<KeyEventArgs>(TextInputKeyPress);
        }

        private void TextInputKeyPress(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                StopTextInput();

            if (e.Key == Key.Enter)
            {
                googleAssistant.Write(TextInputText);
                StopTextInput();
            }
        }

        private void TriggerTextInput()
        {
            if (TextInput)
                StopTextInput();
            else
                StartTextInput();
        }

        private void TriggerMicrophone()
        {
            if (Listening)
                StopListen();
            else
                StartListen();
        }

        private void StartTextInput()
        {
            TextInput = true;
            TextInputOpacity = 1;
        }

        private void StopTextInput()
        {
            Text = TextInputText;
            TextInputText = "";
            TextInput = false;
            TextInputOpacity = 0.3;
        }

        private void StartListen()
        {
            Listening = true;
            MicrophoneOpacity = 1;
            googleAssistant.StartListen();
        }

        private void StopListen()
        {
            Listening = false;
            MicrophoneOpacity = 0.3;
            googleAssistant.StopListen();
        }
    }
}