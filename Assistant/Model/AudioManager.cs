using Assistant.Extensions;
using NAudio.Wave;
using Serilog;
using System;
using System.IO;

namespace Assistant.Model
{
    public sealed class AudioManager : IDisposable
    {
        public const int SAMPLE_RATE_HZ = 16000;

        public MemoryStream OutputStream;

        public event EventHandler<bool> AudioPlaybackChanged;
        public event EventHandler<byte[]> RecordingDataReceived;
        public event EventHandler RecordingStopped;

        /// <summary>
        /// Gets or set the Volume Percentage.
        /// Valid values are between 1 and 100
        /// </summary>
        public int VolumePercentage
        {
            get
            {
                return volumePercentage;
            }
            set
            {
                if (volumePercentage.Equals(value))
                    return;

                value = value.Clamp(1, 100);

                volumePercentage = value;
                Log.Information($"Set Volumen to {volumePercentage}%");
            }
        }

        private int volumePercentage;
        private WaveInEvent recorder;
        private WaveOutEvent player;
        private BufferedWaveProvider audioBuffer;
        private RawSourceWaveStream playerStream;
        private readonly WaveFormat WaveFormat = new WaveFormat(SAMPLE_RATE_HZ, 1);

        /// <summary>
        /// Snapshot of <see cref="OutputStream"/> for playing
        /// </summary>
        private MemoryStream outputStream;

        public AudioManager()
        {
            volumePercentage = 100;
            OutputStream = new MemoryStream();
            outputStream = new MemoryStream();

            recorder = new WaveInEvent()
            {
                WaveFormat = WaveFormat
            };
            recorder.DataAvailable += Recorder_DataAvailable;
            recorder.RecordingStopped += Recorder_RecordingStopped;

            player = new WaveOutEvent();
            player.PlaybackStopped += Player_PlaybackStopped;

            audioBuffer = new BufferedWaveProvider(recorder.WaveFormat);
        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            RecordingStopped?.Invoke(this, EventArgs.Empty);
        }

        private void Player_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            playerStream?.Dispose();
            playerStream = null;

            OutputStream?.Dispose();
            OutputStream = new MemoryStream();

            AudioPlaybackChanged?.Invoke(this, false);
        }

        public void Play()
        {
            Log.Information("Playing Output");
            if (OutputStream.Length < 1)
            {
                Log.Warning("Output has no content");
                return;
            }

            OutputStream.CopyTo(outputStream);
            playerStream = new RawSourceWaveStream(outputStream, WaveFormat);
            player.Init(playerStream);
            player.Play();

            AudioPlaybackChanged?.Invoke(this, true);
        }

        public void StopPlaying()
        {
            player.Stop();
        }

        public void StartRecording()
        {
            recorder.StartRecording();
        }

        public void StopRecording()
        {
            recorder.StopRecording();
        }

        private void Recorder_DataAvailable(object sender, WaveInEventArgs e)
        {
            RecordingDataReceived?.Invoke(this, e.Buffer);
        }

        public void Dispose()
        {
            audioBuffer?.ClearBuffer();
            audioBuffer = null;

            recorder?.Dispose();
            recorder = null;

            player?.Dispose();
            player = null;

            playerStream?.Dispose();
            playerStream = null;

            outputStream?.Dispose();
            outputStream = null;

            OutputStream?.Dispose();
            OutputStream = null;
        }
    }
}
