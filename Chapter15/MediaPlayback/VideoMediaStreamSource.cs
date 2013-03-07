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

namespace MediaPlayback
{
    public class VideoMediaStreamSource : MediaStreamSource
    {
        private const int BytesPerPixel = 4;
        private readonly int _frameWidth;
        private readonly int _frameHeight;
        private readonly int _framePixelCount;
        private readonly int _frameBufferSize;
        private readonly int _frameRate;
        private int _currentBufferFrame = 0;
        private int _currentReadyFrame = 1;
        private int _currentVideoTimeStamp = 0; 
        private int _samplesProvided = 0;

        private byte[][] _frames = new byte[2][];

        private MediaStreamDescription _videoDesc;
        private readonly Dictionary<MediaSampleAttributeKeys, string> _sampleAttr;

        public VideoMediaStreamSource(int frameWidth, int frameHeight)
        {
            System.Diagnostics.Debug.WriteLine("constructor");
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _framePixelCount = frameWidth * frameHeight;
            _frameBufferSize = _framePixelCount * BytesPerPixel;

            _frameRate = (int)TimeSpan.FromSeconds((double)1 / 10).Ticks;

            _frames[0] = new byte[_frameBufferSize];
            _frames[1] = new byte[_frameBufferSize];
            
            _sampleAttr = new Dictionary<MediaSampleAttributeKeys, string>();
            _sampleAttr[MediaSampleAttributeKeys.FrameHeight]
                = _frameHeight.ToString();
            _sampleAttr[MediaSampleAttributeKeys.FrameWidth]
                = _frameWidth.ToString();
            _sampleAttr[MediaSampleAttributeKeys.KeyFrameFlag] = "True";

            FillFrame(0, Colors.Orange);
            FillFrame(1, Colors.Blue);
        }

        public void FillFrame(int index, Color color)
        {
            for (int i = 0; i < _framePixelCount; i++)
            {
                int offset = i * BytesPerPixel;

                _frames[index][offset++] = color.B;
                _frames[index][offset++] = color.G;
                _frames[index][offset++] = color.R;
                _frames[index][offset++] = color.A;
            }
        }

        protected override void OpenMediaAsync()
        {
            System.Diagnostics.Debug.WriteLine("Open");
            Dictionary<MediaSourceAttributesKeys, string> sourceAttributes =
                new Dictionary<MediaSourceAttributesKeys, string>();
            List<MediaStreamDescription> availableStreams =
                new List<MediaStreamDescription>();

            PrepareVideo();

            availableStreams.Add(_videoDesc);

            sourceAttributes[MediaSourceAttributesKeys.Duration] = "0";
            sourceAttributes[MediaSourceAttributesKeys.CanSeek] = "False";

            ReportOpenMediaCompleted(sourceAttributes, availableStreams);
        }

        private void PrepareVideo()
        {
            Dictionary<MediaStreamAttributeKeys, string> attr =
                new Dictionary<MediaStreamAttributeKeys, string>();
            attr[MediaStreamAttributeKeys.VideoFourCC] = "RGBA";
            attr[MediaStreamAttributeKeys.Height] = _frameHeight.ToString();
            attr[MediaStreamAttributeKeys.Width] = _frameWidth.ToString();

            MediaStreamDescription msd = new MediaStreamDescription(
                MediaStreamType.Video, attr);
            _videoDesc = msd;
        }

        protected override void GetSampleAsync(MediaStreamType mediaStreamType)
        {
            if (mediaStreamType != MediaStreamType.Video)
                throw new NotSupportedException();
            
            MemoryStream frameStream = new MemoryStream();
            frameStream.Write(_frames[_currentReadyFrame], 0,
                    _frameBufferSize);

            MediaStreamSample msSamp = new MediaStreamSample(
                _videoDesc, frameStream, 0, _frameBufferSize,
                _currentVideoTimeStamp, _sampleAttr);

            _currentVideoTimeStamp += _frameRate;
            if ((_currentVideoTimeStamp % 10000000) == 0)
            {
                int f = _currentBufferFrame;
                _currentBufferFrame = _currentReadyFrame;
                _currentReadyFrame = f;
                _samplesProvided++;
            }

            if (_samplesProvided < 10)
            {
                ReportGetSampleCompleted(msSamp);
            }
            else
            {
                MediaStreamSample nullSample = new MediaStreamSample(
                    _videoDesc, null, 0, 0, _currentVideoTimeStamp,
                    _sampleAttr);
                ReportGetSampleCompleted(nullSample);
            }
            
        }

        protected override void CloseMedia() {
            
        }

        protected override void SeekAsync(long seekToTime)
        {
            if (seekToTime != 0) throw new NotSupportedException();
            _currentBufferFrame = 0;
            _currentReadyFrame = 1;
            _currentVideoTimeStamp = 0;
            _samplesProvided = 0;
            ReportSeekCompleted(seekToTime);
        }

        protected override void SwitchMediaStreamAsync(
            MediaStreamDescription mediaStreamDescription)
        {
            System.Diagnostics.Debug.WriteLine("switch");
            throw new NotImplementedException();
        }

        protected override void GetDiagnosticAsync(
            MediaStreamSourceDiagnosticKind diagnosticKind)
        {
            System.Diagnostics.Debug.WriteLine("diag");
            throw new NotImplementedException();
        }
    }
}
