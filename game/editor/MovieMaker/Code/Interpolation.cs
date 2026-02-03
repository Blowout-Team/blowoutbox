using System.Numerics;
using Sandbox.MovieMaker;
using Sandbox.Utility;

namespace Editor.MovieMaker;

#nullable enable

public enum InterpolationMode
{
	[Title( "None" )]
	None,

	[Title( "Linear" )]
	Linear,

	[Title( "Ease In" )]
	QuadraticIn,

	[Title( "Ease Out" )]
	QuadraticOut,

	[Title( "Ease In Out" )]
	QuadraticInOut
}

public static class InterpolationExtensions
{
	public static float Apply( this InterpolationMode interpolation, float t ) => interpolation switch
	{
		InterpolationMode.Linear => t,
		InterpolationMode.QuadraticIn => Easing.QuadraticIn( t ),
		InterpolationMode.QuadraticOut => Easing.QuadraticOut( t ),
		InterpolationMode.QuadraticInOut => Easing.QuadraticInOut( t ),
		_ => 0f
	};
}

public interface ITransformer<T>
{
	T Identity => default!;

	T Invert( T value );
	T Apply( T outer, T inner );

	public T Difference( T from, T to ) => Apply( Invert( from ), to );
}

public static class Transformer
{
	private static Dictionary<Type, ITransformer<object?>?> Cache { get; } = new();

	public static ITransformer<object?>? GetDefault( Type type )
	{
		if ( Cache.TryGetValue( type, out var cached ) ) return cached;

		try
		{
			var transformerType = typeof(LocalTransformerWrapper<>)
				.MakeGenericType( type );

			return Cache[type] = (ITransformer<object?>?)Activator.CreateInstance( transformerType );
		}
		catch
		{
			return Cache[type] = null;
		}
	}

	public static ITransformer<T>? GetDefault<T>()
	{
		// TODO: type library lookup?

		return DefaultTransformer.Instance as ITransformer<T>;
	}

	public static ITransformer<T> GetDefaultOrThrow<T>() =>
		GetDefault<T>() ?? throw new Exception( $"Type {typeof(T)} can't be transformed to local." );
}

file interface INumericTransformer<T> : ITransformer<T>
	where T : INumber<T>
{
	T ITransformer<T>.Invert( T value ) => -value;
	T ITransformer<T>.Apply( T outer, T inner ) => outer + inner;
}

file sealed class DefaultTransformer :
	INumericTransformer<int>, INumericTransformer<float>, INumericTransformer<double>,
	ITransformer<Sandbox.Vector2>, ITransformer<Sandbox.Vector3>, ITransformer<Sandbox.Vector4>,
	ITransformer<Rotation>, ITransformer<Angles>,
	ITransformer<Color>,
	ITransformer<Transform>
{
	public static DefaultTransformer Instance { get; } = new();

	Rotation ITransformer<Rotation>.Identity => Rotation.Identity;

	public Color Invert( Color value ) => new( -value.r, -value.g, -value.b, -value.a );
	public Color Apply( Color outer, Color inner ) => outer + inner;

	public Angles Invert( Angles value ) => value.ToRotation().Inverse;
	public Angles Apply( Angles outer, Angles inner ) => outer.ToRotation() * inner.ToRotation();

	public Rotation Invert( Rotation value ) => value.Inverse;
	public Rotation Apply( Rotation outer, Rotation inner ) => outer * inner;

	public Sandbox.Vector4 Invert( Sandbox.Vector4 value ) => -value;
	public Sandbox.Vector4 Apply( Sandbox.Vector4 outer, Sandbox.Vector4 inner ) => outer + inner;

	public Sandbox.Vector3 Invert( Sandbox.Vector3 value ) => -value;
	public Sandbox.Vector3 Apply( Sandbox.Vector3 outer, Sandbox.Vector3 inner ) => outer + inner;

	public Sandbox.Vector2 Invert( Sandbox.Vector2 value ) => -value;
	public Sandbox.Vector2 Apply( Sandbox.Vector2 outer, Sandbox.Vector2 inner ) => outer + inner;

	public Transform Invert( Transform value ) => value.ToLocal( Transform.Zero );
	public Transform Apply( Transform outer, Transform inner ) => outer.ToWorld( inner );
}

file sealed class LocalTransformerWrapper<T> : ITransformer<object?>
{
	private readonly ITransformer<T> _inner;

	public LocalTransformerWrapper()
	{
		_inner = Transformer.GetDefaultOrThrow<T>();
	}

	public object? Invert( object? value )
	{
		return _inner.Invert( (T)value! );
	}

	public object? Apply( object? outer, object? inner )
	{
		return _inner.Apply( (T)outer!, (T)inner! );
	}
}
