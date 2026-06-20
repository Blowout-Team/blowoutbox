using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Core;
using BlowoutTeamSoft.Engine.Exceptions.LowLevel;
using BlowoutTeamSoft.Engine.Interfaces.Audio;
using BlowoutTeamSoft.Engine.Interfaces.Audio.Async;
using Sandbox.Audio;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Sandbox;

[Expose]
[Tint( EditorTint.Green )]
public abstract class BaseSoundComponent : Component, IBlowoutAudioSystem, IBlowoutAudioSource
{
	/// <summary>
	/// The mixer we want this sound to play through
	/// </summary>
	[Property]
	public MixerHandle TargetMixer { get; set; }

	[Property, Group( "Sound" )] public SoundEvent SoundEvent { get; set; }
	[Property, Group( "Sound" )] public bool PlayOnStart { get; set; } = true;
	[Property, Group( "Sound" )] public bool StopOnNew { get; set; } = false;

	[Property, ToggleGroup( "SoundOverride" )] public bool SoundOverride { get; set; } = false;
	[Range( 0, 1 ), Property, Group( "SoundOverride" )] public float Volume { get; set; } = 1.0f;
	[Range( 0, 2 ), Property, Group( "SoundOverride" )] public float Pitch { get; set; } = 1.0f;
	[Property, Group( "SoundOverride" )] public bool Force2d { get; set; } = false;

	[Property, ToggleGroup( "Repeat" )] public bool Repeat { get; set; } = false;
	[Property, Group( "Repeat" )] public float MinRepeatTime { get; set; } = 1.0f;
	[Property, Group( "Repeat" )] public float MaxRepeatTime { get; set; } = 1.0f;

	[Property, ToggleGroup( "DistanceAttenuationOverride", Label = "Override Distance Attenuation" )] public bool DistanceAttenuationOverride { get; set; } = false;
	[Property, Group( "DistanceAttenuationOverride" )] public bool DistanceAttenuation { get; set; } = false;
	[Property, Group( "DistanceAttenuationOverride" ), AudioDistanceFloat] public float Distance { get; set; } = 512f;
	[Property, Group( "DistanceAttenuationOverride" )] public Curve Falloff { get; set; } = new Curve( new( 0, 1, MathF.PI, -MathF.PI ), new( 1, 0 ) );

	[Property, ToggleGroup( "OcclusionOverride", Label = "Override Occlusion" )] public bool OcclusionOverride { get; set; } = false;
	[Property, Group( "OcclusionOverride" )] public bool OcclusionEnabled { get; set; } = false;

	/// <summary>Legacy alias for <see cref="OcclusionEnabled"/>.</summary>
	[Hide, Obsolete( "Use OcclusionEnabled instead." )]
	public bool Occlusion
	{
		get => OcclusionEnabled;
		set => OcclusionEnabled = value;
	}

	/// <summary>Legacy occlusion radius. No longer used by the simulation.</summary>
	[JsonInclude, Hide, Obsolete( "OcclusionRadius is no longer used by the simulation." )]
	public float OcclusionRadius { get; set; } = 32.0f;

	[Property, ToggleGroup( "ReverbOverride", Label = "Override Reverb" )] public bool ReverbOverride { get; set; } = false;
	[Property, Group( "ReverbOverride" )] public bool ReverbEnabled { get; set; } = false;

	/// <summary>Legacy alias for <see cref="ReverbOverride"/>.</summary>
	[Hide, Obsolete( "Use ReverbOverride instead." )]
	public bool ReflectionOverride
	{
		get => ReverbOverride;
		set => ReverbOverride = value;
	}

	/// <summary>Legacy alias for <see cref="ReverbEnabled"/>.</summary>
	[Hide, Obsolete( "Use ReverbEnabled instead." )]
	public bool Reflections
	{
		get => ReverbEnabled;
		set => ReverbEnabled = value;
	}

	protected SoundHandle SoundHandle;

	internal SoundHandle SoundHandleInternal => SoundHandle;

	[IgnoreDataMember, JsonIgnore]
	public string ChannelName => SoundHandle.TargetMixer?.Name ?? "Global Channel";

	[IgnoreDataMember, JsonIgnore]
	public bool IsPlaying => SoundHandle.IsPlaying;

	public bool IsLoop { get => SoundHandle.Loopback; set => SoundHandle.Loopback = value; }

	[IgnoreDataMember, JsonIgnore]
	public TimeSpan CurrentTime { get => TimeSpan.FromSeconds(SoundHandle.Time); set => SoundHandle.Time = (float)value.TotalSeconds; }
	
	[IgnoreDataMember, JsonIgnore]
	public string Name => SoundHandle.Name;

	bool IBlowoutAudioSystem.IsPlaying { get => IsPlaying; set 
		{
			if ( value )
				StartSound();
			else
				Stop();
		}
	
	}
	public IAudioSegment Sound { get => SoundHandle; set => SetAudio(value); }

	public IBlowoutAudioMixer Mixer { get => GetMixer(); set => SetAudioMixer(value); }
	public TimeSpan Time { get => CurrentTime; set => CurrentTime = value; }

	public virtual void StartSound() { }
	public virtual void StopSound() { }

	protected void ApplyOverrides( SoundHandle h )
	{
		if ( !h.IsValid() )
			return;

		h.TargetMixer = TargetMixer.Get( h.TargetMixer );

		if ( SoundOverride )
		{
			h.Volume = Volume;
			h.Pitch = Pitch;
			h.ListenLocal = Force2d;
		}

		if ( OcclusionOverride )
		{
			h.OcclusionEnabled = OcclusionEnabled;
		}

		if ( ReverbOverride )
		{
			h.ReverbEnabled = ReverbEnabled;
		}

		if ( DistanceAttenuationOverride )
		{
			h.DistanceAttenuation = DistanceAttenuation;
			h.Distance = Distance;
			h.Falloff = Falloff;
		}

		if ( Force2d )
		{
			h.Position = Vector3.Forward * 10.0f;
			h.OcclusionEnabled = false;
			h.AirAbsorption = false;
			h.DistanceAttenuation = false;
			h.Transmission = false;
		}
	}

	[Group( "Sound" ), Button( "Test Sound", "play_arrow" ), HideIf( nameof( SoundEvent ), null ), WideMode]
	protected void TestSound()
	{
		StopSound();
		StartSound();
	}

	public void Rewind( TimeSpan time )
	{
		CurrentTime = time;
	}

	public void SetAudio( IAudioSegment audio )
	{
		if(audio is Sandbox.SoundHandle handle )
		{
			SoundHandle = handle;
			return;
		}

		if(audio is SoundFile file )
		{
			SoundHandle = new SoundHandle(file.native);
			return;
		}

		throw new BlowoutUnsupportedException( "Unsupported type of audio mixer for audio segment. Is supports only handles and sound files. Get sound segment type: " + audio.GetType().FullName );
	}

	public void PlayAudio()
	{
		StartSound();
	}

	public void SetAudioMixer( IBlowoutAudioMixer mixer )
	{
		if(mixer is Mixer mix )
		{
			TargetMixer = mix;
			SoundHandle.TargetMixer = mix;
			return;
		}

		throw new BlowoutUnsupportedException("Unsupported type of audio mixer for audio game system: " + mixer.GetType().FullName);
	}

	public void SetAudioChannel( IAudioChannel channel )
	{
		SetAudioMixer( channel.GetMixer() );
	}

	public void ChangeVolume( float value )
	{
		Volume = value;
		SoundHandle?.Volume = value;
	}

	public void ChangePitch( float value )
	{
		Pitch = value;
		SoundHandle?.Pitch = value;
	}

	public void PlayDirectFlow( IAudioSegment segment, float volume = 1 )
	{
		BlowoutEngine.Current.Audio.PlaySound( segment, WorldPosition );
	}

	public void PlayAudio( IAudioSegment audio )
	{
		StopAudio();
		SetAudio( audio );
		StartSound();
	}

	//TODO: Dehs: Make it work. I really dont need it for now :P.
	public void PlayAudio<T>( IAudioSegment audio, IAudioEffect<T> effect ) where T : IAudioChannel
	{
		throw new NotImplementedException();
	}

	public void PlayAudioAsyncEffect( IAudioSegment audio )
	{
		throw new NotImplementedException();
	}

	public void PlayAudioAsyncEffect<T>( IAudioSegment audio, IAudioEffectAsync<T> asyncEffect = null ) where T : IAudioChannel
	{
		throw new NotImplementedException();
	}

	public void StopAudio()
	{
		StopSound();
	}

	public BlowoutEngineGameObject GetSourceObject() =>
		GameObject;

	public IBlowoutAudioMixer GetMixer() =>
		SoundHandle.TargetMixer;

	public void Dispose()
	{
		SoundHandle?.Dispose();
	}

	public void Play()
	{
		PlayAudio();
	}

	public void PlayFlow( IAudioSegment segment )
	{
		BlowoutEngine.Current.Audio.PlaySound( segment, WorldPosition );
	}

	public void PlayFlow( IAudioSegment segment, float volume )
	{
		if ( segment is SoundHandle handle )
			handle.Volume = volume;

		BlowoutEngine.Current.Audio.PlaySound( segment, WorldPosition );
	}

	public void Stop()
	{
		StopAudio();
	}
}

