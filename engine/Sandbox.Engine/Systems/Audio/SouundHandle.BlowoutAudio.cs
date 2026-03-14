using BlowoutTeamSoft.Engine.Exceptions;
using BlowoutTeamSoft.Engine.Interfaces.Audio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using static Sandbox.Tasks.ExpirableSynchronizationContext;
using static Sandbox.VertexLayout;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sandbox;

unsafe partial class SoundHandle : IAudioSegment
{
	public float[] Segments
	{
		get
		{
			int sampleCount = _sfx.GetSampleCount();
			if ( sampleCount == 0 )
			{
				return null;
			}

			var samples = new short[sampleCount];

			fixed ( short* memory = &samples[0] )
			{
				if ( !_sfx.GetSamples( (IntPtr)memory, (uint)sampleCount ) )
					return null;
			}

			return samples.Select(x=> (float)x).ToArray();
		}
	}

	public int Samples => SampleRate;

	public int Channels => sampler.GetLastReadSamples().ChannelCount;

	public int Frequency => SampleRate;

	public float Length { get 
		{
			if ( sampler is null ) return 0.0f;

			return SampleRate > 0 ? sampler.SamplePosition / (float)SampleRate : 0.0f;
		}
		set => throw new BlowoutTeamSoft.Engine.Exceptions.BlowoutEngineException("Can not edit length of sound"); }

	TimeSpan IAudioSegment.Time => TimeSpan.FromSeconds( Time );

	public unsafe void SetSegments( float[] segments, int offset ) =>
		throw new BlowoutEngineException("Source 2 Can not set segments into cached sound handle. Use creating of sound insted SetSegments in this Backend Framework.");
}
