using BlowoutTeamSoft.Engine.Core;
using BlowoutTeamSoft.Engine.Exceptions;
using BlowoutTeamSoft.Engine.Interfaces.Audio;
using BlowoutTeamSoft.Engine.Interfaces.Audio.Async;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox.Audio;

public partial class Mixer : IAudioChannel, IBlowoutAudioMixer
{
	private PitchProcessor _pitchProcessor;
	public float Pitch
	{
		get
		{
			if ( _pitchProcessor == null )
				return 0;

			return _pitchProcessor.Pitch;
		}
		set
		{
			_pitchProcessor ??= new PitchProcessor();
			AddProcessor( _pitchProcessor );
			_pitchProcessor.Pitch = value;
		}
	}

	protected SoundHandle _currentHandle;

	public IEnumerable<IBlowoutAudioMixer> Childs
	{
		get
		{
			foreach(var child in Children )
			{
				yield return child;
			}
		}
	}

	public IAudioChannel Channel => this;

	public IBlowoutAudioMixerParameters Parameters => throw new NotImplementedException();

	public void AddSubMixer( IBlowoutAudioMixer mixer )
	{
		if ( mixer is not Mixer sourceMixer )
			throw new BlowoutEngineArgumentException(nameof(mixer), mixer);
		lock ( Lock )
		{
			Children.Add( sourceMixer );
		}
	}

	public void ChangePitch( float value )
	{
		if ( value == Pitch)
			return;

		Pitch = value;
	}

	public void ChangeVolume( float value )
	{
		Volume = value;
	}

	public IBlowoutAudioMixer CreateMixer( string name ) =>
		new Mixer( this ) { Name = name };

	public IBlowoutAudioMixer GetMixer() =>
		this;

	public IBlowoutAudioMixer GetMixer( string name ) =>
		Children.FirstOrDefault( x => x.Name == name );

	public BlowoutEngineGameObject GetSourceObject()
	{
		throw new NotImplementedException();
	}

	public void PlayAudio( IAudioSegment audio )
	{
		_currentHandle?.Stop();
		if(audio is SoundHandle sound )
		{
			sound.TargetMixer = this;
			if ( sound.IsPlaying )
				return;
			_currentHandle = sound;
		}
	}

	public void PlayAudio<T>( IAudioSegment audio, IAudioEffect<T> effect ) where T : IAudioChannel
	{
		PlayAudio( audio );
		if(this is T target)
			effect.Apply( target );
	}

	public void PlayAudioAsyncEffect( IAudioSegment audio )
	{
		PlayAudio( audio );
	}

	public void PlayAudioAsyncEffect<T>( IAudioSegment audio, IAudioEffectAsync<T> asyncEffect = null ) where T : IAudioChannel
	{
		PlayAudio( audio );
		if ( this is T target )
			asyncEffect.ApplyAsync( target );
	}

	public void PlayDirectFlow( IAudioSegment segment, float volume = 1 )
	{
		if ( segment is SoundHandle handle )
		{
			handle.Volume = volume;
			if ( handle.Paused )
				handle.Paused = false;
		}
	}

	public void RemoveSubMixer( IBlowoutAudioMixer mixer )
	{
		Children.RemoveAll( x => x.Name == mixer.Name );
	}

	public void StopAudio()
	{
		StopAll(0.3f);
	}

	public void Dispose()
	{
		_pitchProcessor?.Dispose();
		Destroy();
		GC.SuppressFinalize( this );
	}
}
