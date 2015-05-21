using System;
using System.IO;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using BoonieBear.DeckUnit.VoiceManager;
namespace BoonieBear.DeckUnit.WaveBox
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class WaveControl : UserControl, IDisposable
    {
        private string title = "波形显示";
        private WaveInRecorder _recorder;
        private byte[] _recorderBuffer;
        private WaveOutPlayer _player;
        private byte[] _playerBuffer;
        private FifoStream _stream;
        private WaveFormat _waveFormat;
        private AudioFrame _audioFrame;
        private int _audioSamplesPerSecond = 8000;
        private int _displayFrequecyMax = 4000;
        private int _displayAmpMax = 32767;
        private int _TimeDomainLen = 16000;
        private int _audioFrameSize = 1024;
        private int _audioBitsPerSample = 16;
        private int _audioChannels = 1;
        private BinaryReader br;
        private bool _isPlayer = true;
        private DispatcherTimer timer;
        public enum DisplayType
        {
            WAVE,
            SPECTRUM
        }
        private DisplayType _type = DisplayType.WAVE;
        
        public WaveControl()
        {
            InitializeComponent();
            Initailize();
            
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
        
            Stop();
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
        public bool isPlaying
        {
            set
            {
                _isPlayer = value;
                try
                {
                    if (_isPlayer)
                    {
                        _waveFormat = new WaveFormat(_audioSamplesPerSecond, _audioBitsPerSample, _audioChannels);

                        if (WaveOutPlayer.DeviceCount == 0)
                        {

                            _isPlayer = false;
                            throw new Exception("没有可用的音频设备");
                        }
                        else
                        {
                           _player = new WaveOutPlayer(-1, _waveFormat, _audioFrameSize*2, 3,
                                    new BufferFillEventHandler(Filler));
                        }
                    }
                    else
                    {
                        if(_player!=null)
                            _player.Dispose();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            get { return _isPlayer; }
        }

        public DisplayType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                if (_type == DisplayType.SPECTRUM)
                    _audioFrame.ShowFFT = true;
                else
                    _audioFrame.ShowFFT = false;
            }
        }

 
        public void Initailize()
        {
            Stop();
            _stream = new FifoStream();
            _audioFrame = new AudioFrame(_audioSamplesPerSecond, _displayFrequecyMax, _TimeDomainLen, _displayAmpMax, _audioBitsPerSample, Type == DisplayType.SPECTRUM);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 128);
            timer.Tick += timer_Tick;

            try
            {
                _waveFormat = new WaveFormat(_audioSamplesPerSecond, _audioBitsPerSample, _audioChannels);  
                if (WaveOutPlayer.DeviceCount == 0)
                {
                        
                    isPlaying = false;
                    throw new Exception("没有可用的音频设备");
                }
                else
                {
                    if (_isPlayer)
                        _player = new WaveOutPlayer(-1, _waveFormat, _audioFrameSize*_audioChannels, 3, new BufferFillEventHandler(Filler));
                }

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
        private void Stop()
        {
            isPlaying = false;
            if (_recorder != null)
                try
                {
                    _recorder.Dispose();
                }
                finally
                {
                    _recorder = null;
                }
            if (_isPlayer == true)
            {
                if (_player != null)
                    try
                    {
                        _player.Dispose();
                    }
                    finally
                    {
                        _player = null;
                    }

                if (_stream != null)
                    try
                    {
                        _stream.Flush(); // clear all pending data
                    }
                    finally
                    {
                        _stream = null;
                    }
                
            }
        }

        private void Filler(IntPtr data, int size)
        {
            if (_isPlayer == true)
            {
                if (_playerBuffer == null || _playerBuffer.Length < size)
                    _playerBuffer = new byte[size];
                if (_stream != null && _stream.Length >= size)
                    _stream.Read(_playerBuffer, 0, size);
                else
                    for (int i = 0; i < _playerBuffer.Length; i++)
                        _playerBuffer[i] = 0;
                System.Runtime.InteropServices.Marshal.Copy(_playerBuffer, 0, data, size);
            }
        }

        private void DataArrived(IntPtr data, int size)
        {
            if (_recorderBuffer == null || _recorderBuffer.Length < size)
                _recorderBuffer = new byte[size];
            if (_recorderBuffer != null)
            {
                System.Runtime.InteropServices.Marshal.Copy(data, _recorderBuffer, 0, size);
                if (_isPlayer == true)
                    _stream.Write(_recorderBuffer, 0, _recorderBuffer.Length);
                byte[] b = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(data,b,0, size);
                try
                {
                    //_audioFrame.Process(ref b);
                    //_audioFrame.SetYAxis(ref YSection);
                    _audioFrame.RenderTimeDomain(ref ImageBox);
                    //_audioFrame.RenderSpectrogram(ref FrequencyDomainBox);
                }
                catch (Exception MyEx)
                {
                    //do nothing
                }
            }
        }
        public void Display(byte[] buf)
        {
            if (_isPlayer == true)
                _stream.Write(buf, 0, buf.Length);
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
                if (buf.Length - size!=0)
                {
                    byte[] lastbuf = new byte[buf.Length - size];
                    Array.Copy(buf, size, lastbuf, 0, buf.Length - size);
                    _audioFrame.AddData(lastbuf);
                }

            }
            

        }
        
        private void ImageBox_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (_audioFrame != null)
            {
                
                //_audioFrame.SetYAxis(ref YSection);
                if (_type == DisplayType.WAVE)
                    _audioFrame.RenderTimeDomain(ref ImageBox);
                if (_type == DisplayType.SPECTRUM)
                    _audioFrame.RenderSpectrogram(ref ImageBox);

            }
        }

        private void ImageBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (_audioFrame != null)
            {
                //_audioFrame.SetYAxis(ref YSection);
                if (_type == DisplayType.WAVE)
                    _audioFrame.RenderTimeDomain(ref ImageBox);
                if (_type == DisplayType.SPECTRUM)
                    _audioFrame.RenderSpectrogram(ref ImageBox);

            }

        }


    }
}
