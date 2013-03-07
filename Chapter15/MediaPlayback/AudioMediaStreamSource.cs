using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using MediaPlayback;

namespace MediaPlayback
{
    public class AudioMediaStreamSource : MediaStreamSource
    {
        private MediaStreamDescription _audioDesc;
        private long _currentTimeStamp;

        private readonly int _numSamples;
        private readonly  int _bufferByteCount;
        private readonly long _audioDuration;
        private readonly string _encodedWaveFormat;
        
        private readonly Dictionary<MediaSampleAttributeKeys, string> _sampleAttr
            = new Dictionary<MediaSampleAttributeKeys, string>();
        
        private readonly SineWaveformOscillator _oscillator;

        public AudioMediaStreamSource(short frequency)
        {
            var waveFormat = new WaveFormatEx()
            {
                SamplesPerSec = 44100,
                Channels = 1,
                BitsPerSample = 16,
                AvgBytesPerSec = 44100 * 2,
                BlockAlign = 2,
                FormatTag = WaveFormatEx.FormatPCM,
                Size = 0
            };
            waveFormat.ValidateWaveFormat();
                        
            _numSamples = waveFormat.Channels * 256;
            _bufferByteCount = waveFormat.BitsPerSample / 8 * _numSamples;
            _audioDuration = waveFormat.AudioDurationFromBufferSize((uint)_bufferByteCount);
            _encodedWaveFormat = waveFormat.ToHexString();
            
            _oscillator = new SineWaveformOscillator()
            {
                Frequency = frequency * 2
            };
        }

        protected override void OpenMediaAsync()
        {
            Dictionary<MediaStreamAttributeKeys, string> streamAttr =
                new Dictionary<MediaStreamAttributeKeys, string>();
            streamAttr[MediaStreamAttributeKeys.CodecPrivateData] = _encodedWaveFormat;

            _audioDesc = new MediaStreamDescription( MediaStreamType.Audio, streamAttr);

            List<MediaStreamDescription> availableStreams =
                new List<MediaStreamDescription>();
            availableStreams.Add(_audioDesc);

            Dictionary<MediaSourceAttributesKeys, string> sourceAttr =
                new Dictionary<MediaSourceAttributesKeys, string>();
            sourceAttr[MediaSourceAttributesKeys.Duration] = "0";
            sourceAttr[MediaSourceAttributesKeys.CanSeek] = "False";

            ReportOpenMediaCompleted(sourceAttr, availableStreams);
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            if (mediaStreamType != MediaStreamType.Audio)
                throw new NotSupportedException();

            MemoryStream stream = new MemoryStream();
            for (int i = 0; i < _numSamples; i++)
            {
                short sample = _oscillator.GetNextSample();
                stream.WriteByte((byte)(sample & 0xff));
                stream.WriteByte((byte)(sample >> 8));
            }

            MediaStreamSample streamSample = new MediaStreamSample(_audioDesc,
                stream, 0, _bufferByteCount,
                _currentTimeStamp, _sampleAttr);

            _currentTimeStamp += _audioDuration;

            ReportGetSampleCompleted(streamSample);
        }

        protected override void SeekAsync(long seekToTime)
        {
            if (seekToTime != 0) throw new NotSupportedException();
            _currentTimeStamp = 0;
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(
            MediaStreamDescription mediaStreamDescription)
        {
            throw new NotImplementedException();
        }

        protected override void CloseMedia()
        {
            //_startPosition = _currentPosition = 0;
            //_audioDesc = null;
        }

        protected override void GetDiagnosticAsync(
            MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            throw new NotImplementedException();
        }
    }
}
