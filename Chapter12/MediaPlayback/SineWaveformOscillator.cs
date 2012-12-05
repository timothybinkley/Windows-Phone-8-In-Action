using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPlayback
{
    public class SineWaveformOscillator
{
    double frequency;
    uint phaseAngleIncrement;
    uint phaseAngle = 0;

    public double Frequency
    {
        set
        {
            frequency = value;
            phaseAngleIncrement = 
                (uint)(frequency * uint.MaxValue / 44100); 
        }
        get
        {
            return frequency;
        }
    }

    public short GetNextSample()
    {
        ushort wholePhaseAngle = (ushort)(phaseAngle >> 16);
        short amplitude = (short)(short.MaxValue *                      
            Math.Sin(2 * Math.PI * wholePhaseAngle / ushort.MaxValue)); 
        phaseAngle += phaseAngleIncrement;
        return amplitude;
    }
}
}
