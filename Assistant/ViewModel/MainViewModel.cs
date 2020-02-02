using Assistant.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows;
using System.Windows.Input;

namespace Assistant.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        #region UI
        public double MicrophoneOpacity { get; set; }

        public double TextInputOpacity { get; set; }

        public bool Listening { get; set; }

        public bool TextInput { get; set; }

        public string TextInputText { get; set; }

        public string Text { get; set; }

        public ICommand TriggerMicrophoneCommand { get; set; }

        public ICommand TriggerTextInputCommand { get; set; }

        public ICommand TextInputKeyPressCommand { get; set; }

        public ICommand LoadedCommand { get; set; }
        #endregion

        private MyAssistant myAssistant;

        public MainViewModel()
        {
            Text = "whats up?";
            MicrophoneOpacity = 0.3;
            TextInputOpacity = 0.3;
            TextInput = false;
            Listening = false;
            TriggerMicrophoneCommand = new RelayCommand(TriggerMicrophone);
            TriggerTextInputCommand = new RelayCommand(TriggerTextInput);
            TextInputKeyPressCommand = new RelayCommand<KeyEventArgs>(TextInputKeyPress);
            LoadedCommand = new RelayCommand(Loaded);

            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }
        }

        public async void Loaded()
        {
            myAssistant = new MyAssistant();
            myAssistant.WaitingForFollowUp += MyAssistant_WaitingForFollowUp;
            myAssistant.SpeechRecognized += MyAssistant_SpeechRecognized;
            myAssistant.DisplayText += MyAssistant_DisplayText;
            myAssistant.DisplayHtml += MyAssistant_DisplayHtml;

            await myAssistant.Initialize();
        }

        private void MyAssistant_DisplayHtml(object sender, string e)
        {
            Text = e;
        }

        private void MyAssistant_DisplayText(object sender, string e)
        {
            Text = e;
        }

        private void MyAssistant_SpeechRecognized(object sender, MyAssistant.SpeechRecognitionResult e)
        {
            Text = e.ToString();
        }

        private void MyAssistant_WaitingForFollowUp(object sender, MyAssistant.FollowUpEventArgs e)
        {
            e.Text = "egal";
        }

        private async void TextInputKeyPress(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                StopTextInput();

            if (e.Key == Key.Enter)
            {
                StopTextInput();

                await myAssistant.NewTextConversation(Text, true);
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

        private async void StartListen()
        {
            Listening = true;
            MicrophoneOpacity = 1;

            await myAssistant.NewAudioConversation(true);
        }

        private void StopListen()
        {
            Listening = false;
            MicrophoneOpacity = 0.3;

            myAssistant.StopStreamingAudio();
        }
    }
}
