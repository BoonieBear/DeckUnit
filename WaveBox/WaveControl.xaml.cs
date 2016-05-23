using System;
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using BoonieBear.DeckUnit.VoiceManager;
using System.Threading;
namespace BoonieBear.DeckUnit.WaveBox
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class WaveControl : UserControl, IDisposable
    {
        private string title = "波形显示";
        private WaveIn _recorder;
        private byte[] _recorderBuffer;
        private WaveOut _player;
        private byte[] _playerBuffer;
        private FifoStream _stream;
        private AudioFrame _audioFrame;
        private int _audioSamplesPerSecond = 8000;
        private int _displayFrequecyMax = 4000;
        private int _displayAmpMax = 32767;
        private int _TimeDomainLen = 8192;
        private int _audioFrameSize = 1024;
        private int _audioBitsPerSample = 16;
        private int _audioChannels = 1;
        private Thread _playSound;
        private DispatcherTimer timer;
        public delegate void Recordbufferdonehanlder(byte[] bufBytes);

        private Recordbufferdonehanlder _recProc;
        public void AddRecDoneHandle(Recordbufferdonehanlder AttachProc)
        {
            _recProc = AttachProc;
        }

        public void RemoveRecDoneHandle()
        {
            _recProc = null;
        }
        public enum DisplayType
        {
            WAVE,
            SPECTRUM
        }
        private DisplayType _displayType = DisplayType.WAVE;
        
        public WaveControl()
        {
            InitializeComponent();
        }

        private void MouseButtonDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (Type == DisplayType.WAVE)
            {

                if (e.ChangedButton == MouseButton.Left)
                {
                    if (MaxAmp <= 4096)
                        MaxAmp = 32767;
                    else
                        MaxAmp = MaxAmp/2;

                }
                if (e.ChangedButton == MouseButton.Right)
                {
                    Type = DisplayType.SPECTRUM;
                    
                    ImageBox.Source = null;
                }
            }
            else
            {
                if (e.ChangedButton == MouseButton.Right)
                {
                    Type = DisplayType.WAVE;
                    ImageBox.Source = null;
                }
                    
            }
        }
        
        public void Dispose()
        {
            if(IsPlaying)
                StopPlaying();
            if(IsRecording)
                StopRecoding();
            if (_stream != null)
            {
                try
                {
                    _stream.Flush(); // clear all pending data
                }
                finally
                {
                    _stream = null;
                }
            }
            if (_player != null)
            {
                try
                {
                    _player.Dispose(); 
                }
                finally
                {
                    _player = null;
                }
            }
            if (_recorder != null)
            {
                try
                {
                    _recorder.Dispose(); 
                }
                finally
                {
                    _recorder = null;
                }
            }
            GC.SuppressFinalize(this);
        }
        public string Title
        {
            set
            {
                title = value;
            }
            get { return title; }
        }
        public int SamlesPerSecond
        {
            set
            {
                _audioSamplesPerSecond = value;
            }
            get { return _audioSamplesPerSecond; }
        }
        public int MaxFrequecyShow
        {
            set
            {
                _displayFrequecyMax = value;
            }
            get { return _displayFrequecyMax; }
        }
        public int MaxAmp
        {
            set
            {
                _displayAmpMax = value;
                _audioFrame.Ymax = value;
            }
            get { return _displayAmpMax; }
        }
        public int TimeWindowSamples
        {
            set
            {
                _TimeDomainLen = value;
            }
            get { return _TimeDomainLen; }
        }
        public int AudioFrameSize
        {
            set
            {
                _audioFrameSize = value;
            }
            get { return _audioFrameSize; }
        }

        public int BitsPerSample
        {
            set
            {
                _audioBitsPerSample = value;
            }
            get { return _audioBitsPerSample; }
        }
        public int Channel
        {
            set
            {
                _audioChannels = value;
            }
            get { return _audioChannels; }
        }

        public DisplayType Type
        {
            get { return _displayType; }
            set
            {
                _displayType = value;
                if (_displayType == DisplayType.SPECTRUM)
                    _audioFrame.ShowFFT = true;
                else
                    _audioFrame.ShowFFT = false;
            }
        }

        public bool IsPlaying { get; private set; }
        public bool IsRecording { get; private set; }

        /// <summary>
        /// start a thread to listen the fifo input
        /// </summary>
        public void StartPlaying()
        {
            _playSound = new Thread(new ThreadStart(playSound));
            _playSound.IsBackground = true;
            _playSound.Start();
        }

        private void playSound()
        {
            if (_playerBuffer == null)
                    _playerBuffer = new byte[4096];
            IsPlaying = true;
 	        try
            {
                while (IsPlaying)
                {
                    if (_stream != null)
                    {
                        int ret = _stream.Read(_playerBuffer, 0, 4096);
                        if(ret>0)
                            _player.Play(_playerBuffer, 0, ret);
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch
            {
               
            }
        }
        /// <summary>
        /// start reording voice
        /// </summary>
        public void StartRecording()
        {
            _recorder.Start();
            IsRecording = true;
        }
        public void Initailize()
        {
            if (_stream==null)
                _stream = new FifoStream();
            if (_audioFrame==null)
            _audioFrame = new AudioFrame(_audioSamplesPerSecond, _displayFrequecyMax, _TimeDomainLen, _displayAmpMax, _audioBitsPerSample, Type == DisplayType.SPECTRUM);
            if (timer==null)
                timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 64);
            timer.Tick -= timer_Tick;
            timer.Tick += timer_Tick;
            timer.Start();
            try
            {
                if (_recorder != null || _player != null)
                {
                    throw new Exception("please stop first");
                }

                int waveInDevice = (Int32)System.Windows.Forms.Application.UserAppDataRegistry.GetValue("WaveIn", 0);
                int waveOutDevice = (Int32)System.Windows.Forms.Application.UserAppDataRegistry.GetValue("WaveOut", 0);

                _recorder = new WaveIn(WaveIn.Devices[waveInDevice], _audioSamplesPerSecond, _audioBitsPerSample, _audioChannels, 800);
                _recorder.BufferFull += new BufferFullHandler(DataArrived);

                _player = new WaveOut(WaveOut.Devices[waveOutDevice], _audioSamplesPerSecond, _audioBitsPerSample, _audioChannels);
                IsRecording = false;
                IsPlaying = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Clear()
        {
            ImageBox.Source = null;

        }
        public void StopPlaying()
        {
            if (_playSound != null)
            {
                try
                {
                    IsPlaying = false;
                    _playSound.Abort();
                    
                    _stream.Flush();
                }
                catch (ThreadAbortException)
                {
                }
                catch
                {
                    
                }
            }
        }
        public void StopRecoding()
        {
            if (_recorder != null)
            {
                try
                {
                    _recorder.Stop();
                    IsRecording = false;
                }
                catch
                { }
            }
        }

        private void DataArrived(byte[] buffer)
        {
                try
                {
                    if (_recProc != null)
                        _recProc(buffer);
                }
                catch (Exception MyEx)
                {
                    //do nothing
                }

        }
        public void Display(byte[] buf)
        {
            try
            {
                if (IsPlaying)
                {
                    _stream.Write(buf, 0, buf.Length);
                }

                if (buf.Length <= _audioFrameSize)
                {
                    _audioFrame.AddData(buf);
                }
                else
                {
                    int size = 0;
                    for (int i = 0; i < Math.Floor((double)buf.Length / _audioFrameSize); i++)
                    {

                        byte[] newbuf = new byte[_audioFrameSize];
                        Array.Copy(buf, i * _audioFrameSize, newbuf, 0, _audioFrameSize);
                        _audioFrame.AddData(newbuf);
                        size += _audioFrameSize;
                    }
                    //最后一包
                    if (buf.Length - size != 0)
                    {
                        byte[] lastbuf = new byte[buf.Length - size];
                        Array.Copy(buf, size, lastbuf, 0, buf.Length - size);
                        _audioFrame.AddData(lastbuf);
                    }

                }

            }
            catch (Exception ex) 
            {
                throw ex;
            }
            
            

        }
        
        private void ImageBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_audioFrame != null)
            {
                
                //_audioFrame.SetYAxis(ref YSection);
                if (_displayType == DisplayType.WAVE)
                    _audioFrame.RenderTimeDomain(ref ImageBox);
                if (_displayType == DisplayType.SPECTRUM)
                    _audioFrame.RenderSpectrogram(ref ImageBox);

            }
        }

        private void ImageBox_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (_audioFrame != null)
            {
                //_audioFrame.SetYAxis(ref YSection);
                if (_displayType == DisplayType.WAVE)
                    _audioFrame.RenderTimeDomain(ref ImageBox);
                if (_displayType == DisplayType.SPECTRUM)
                    _audioFrame.RenderSpectrogram(ref ImageBox);

            }

        }
    }
}
