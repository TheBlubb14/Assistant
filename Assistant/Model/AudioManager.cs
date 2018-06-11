using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assistant.Model
{
    public class AudioManager : IDisposable
    {
        public const int SampleRateHz = 16000;

        private WaveIn recorder;
        private Queue<byte[]> queue;
        private bool IsRecording;

        public AudioManager()
        {
            recorder = new WaveIn();
            recorder.WaveFormat = new WaveFormat(SampleRateHz, 1);
            recorder.DataAvailable += this.Recorder_DataAvailable;
            recorder.RecordingStopped += this.Recorder_RecordingStopped;

            queue = new Queue<byte[]>();
        }

        public void StartRecording()
        {
            IsRecording = true;
            recorder.StartRecording();
        }

        private void Recorder_DataAvailable(object sender, WaveInEventArgs e)
        {
            var buffer = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);

            queue.Enqueue(buffer);

            Debug.WriteLine("DataAvaible: " + buffer.Length);
            Debug.WriteLine("Que: " + queue.Count);
        }

        private void Recorder_RecordingStopped(object sender, StoppedEventArgs e)
        {
            IsRecording = false;
        }

        public void StopRecording()
        {
            recorder.StopRecording();
        }

        public async Task<byte[]> GetBufferAsync()
        {
            while (IsRecording && queue.Count == 0)
                await Task.Delay(10);

            if (!IsRecording && queue.Count == 0)
                return null;

            return queue.Dequeue();
        }


        WaveOut waveOut;
        RawSourceWaveStream stream;

        public void Play(byte[] data)
        {
            waveOut = new WaveOut();
            stream = new RawSourceWaveStream(data, 0, data.Length, recorder.WaveFormat);
            waveOut.PlaybackStopped += this.WaveOut_PlaybackStopped;      
            waveOut.Init(stream);
            waveOut.Volume = 1;
            waveOut.Play();
        }

        public void PlayInternal()
        {
            var buffer = new List<byte>();

            while (queue.Count != 0)
                buffer.AddRange(queue.Dequeue());

            waveOut = new WaveOut();
            stream = new RawSourceWaveStream(buffer.ToArray(), 0, buffer.Count, recorder.WaveFormat);
            waveOut.PlaybackStopped += this.WaveOut_PlaybackStopped;
            waveOut.Init(stream);
            waveOut.Volume = 1;
            waveOut.Play();

        }

        private void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            stream.Dispose();
            waveOut.Dispose();
        }

        public void Dispose()
        {
            IsRecording = false;
            queue.Clear();
            queue = null;

            recorder.StopRecording();
            recorder.Dispose();
            recorder = null;
        }
    }
}
