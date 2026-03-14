using BlowoutTeamSoft.Engine.Interfaces.UI;
using BlowoutTeamSoft.Engine.NativeHandles;
using BlowoutTeamSoft.UI.Interfaces.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sandbox.UI;

public partial class Panel : IBlowoutUINativeVisualPanel
{
	public BlowoutUIElementNativeHandle Handle => new BlowoutUIElementNativeHandle( this );

	public string Name => ElementName;

	public bool IsEnabled { get => HasClass("disabled"); set 
		{
			if ( value )
				RemoveClass( "disabled" );
			else
				AddClass( "disabled" );
		}
	}
	public int DispayId { get => Style.ZIndex.Value; set => Style.ZIndex = value; }

	public IBlowoutUIControlStyle ControlStyle { get => Style; set => Style = (Sandbox.UI.PanelStyle)value; }
	bool IBlowoutUINativeVisualPanel.IsVisible { get => IsVisible; set => IsVisible = value; }

	IBlowoutUINativeVisualPanel IBlowoutUINativeVisualPanel.Parent => Parent;

	IEnumerable<IBlowoutUINativeVisualPanel> IBlowoutUINativeVisualPanel.Children => Children;

	public void AddChild( IBlowoutUINativeVisualPanel panel )
	{
		if(panel is Sandbox.UI.Panel source2Panel)
		{
			AddChild( source2Panel );
			return;
		}

		Log.Error( "It is not possible to add an instance of an object that is not a native Panel of the BlowoutSource2Backend engine." );
	}

	public void ClearChildren()
	{
		DeleteChildren();
	}

	private bool _IsQueryMatch(ReadOnlySpan<char> query, Panel panel )
	{
		if ( query[0] == '#' )
			return panel.Id == query[1..];
		if ( query[0] == '.' )
		{
			Span<Range> ranges = stackalloc Range[10];
			var length = query.Split( ranges, '.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries );

			bool isValid = true;
			for(int i = 0; i < length; i++ )
			{
				int offset = 0;
				if ( query[ranges[i]][0] == '.' )
					offset++;

				if ( !HasClass( query[ranges[i]] ) )
				{
					isValid = false;
					break;
				}
			}

			return isValid;
		}

		if ( panel.GetType().Name == query )
			return true;

		return false;
	}

	public T QueryBlowoutElement<T>( string query ) where T : IBlowoutUINativeVisualPanel
	{
		if ( this is T mainTarget && _IsQueryMatch( query, this ) )
			return mainTarget;

		foreach(var child in Descendants )
		{
			if ( child is T target && _IsQueryMatch( query, child ) )
				return target;
		}

		return default;
	}

	public IBlowoutUINativeVisualPanel QueryBlowoutElement( string query )
	{
		if ( _IsQueryMatch( query, this ) )
			return this;

		foreach ( var child in Descendants )
		{
			if ( _IsQueryMatch( query, child ) )
				return child;
		}

		return default;
	}

	public void RemoveChild( IBlowoutUINativeVisualPanel panel )
	{
		if ( panel is Sandbox.UI.Panel source2Panel )
		{
			RemoveChild( source2Panel );
			return;
		}

		Log.Error( "It is not possible to remove an instance of an object that is not a native Panel of the BlowoutSource2Backend engine." );
	}

	public void RemoveChild( int index )
	{
		if(index >= ChildrenCount )
		{
			Log.Warning($"Trying to remove panel with index '{index}', but it exceeds the size of the child collection. Skip removing." );
			return;
		}

		RemoveChild( Children.ElementAt( index ) );
	}
}
