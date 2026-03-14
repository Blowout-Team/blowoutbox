using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Engine.Exceptions.LowLevel;
using BlowoutTeamSoft.Engine.Input;
using BlowoutTeamSoft.Engine.Interfaces.Input;
using Sandbox.Engine;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using static Sandbox.Services.Inventory;

namespace Sandbox;

/// <summary>
/// An input action defined by a game project.
/// </summary>
[Expose]
public partial class InputAction : IBlowoutInputMap
{
	public bool IsEnabled { get; set; } = true;

	[IgnoreDataMember, JsonIgnore]
	private BlowoutSource2InputCollection _actions
	{
		get
		{
			if ( string.IsNullOrEmpty( Application.GameIdent ) )
				return null;

			field ??= new BlowoutSource2InputCollection( this );

			return field;
		}
	}

	[IgnoreDataMember, JsonIgnore]
	public IEnumerable<IBlowoutInputKeyGroup> Keys
	{
		get
		{
			yield return new BlowoutInputKey( KeyboardCode, BlowoutTeamSoft.Engine.LowLevel.BlowoutOperatingDevice.Windows, _actions);
			yield return new BlowoutInputKey( GamepadCode.ToString(), BlowoutTeamSoft.Engine.LowLevel.BlowoutOperatingDevice.XBox, _actions);

			foreach(var buttonSlot in _actions.Bind.Slots )
			{
				foreach(var slot in buttonSlot.Buttons )
				{
					yield return new BlowoutInputKey( slot, BlowoutTeamSoft.Engine.LowLevel.BlowoutOperatingDevice.Windows, _actions );
				}
			}
		}
	}


	public InputAction( string name, string keyboardCode, GamepadCode gamepadCode = GamepadCode.None, string groupName = "Other", string title = null )
	{
		Name = name;
		KeyboardCode = keyboardCode;
		GamepadCode = gamepadCode;
		GroupName = groupName;
		Title = title;
	}

	public InputAction()
	{
	}

	public InputAction( InputAction other )
	{
		Name = other.Name;
		KeyboardCode = other.KeyboardCode;
		GamepadCode = other.GamepadCode;
		GroupName = other.GroupName;
		Title = other.Title;
	}

	/// <summary>
	/// The name of the input action. Used by Input.Down|Pressed|Released.
	/// </summary>
	[RegularExpression( @"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Lower or upper case letters and underscores, no spaces or other special characters" )]
	[Group( "Display" )]
	public string Name { get; set; }

	/// <summary>
	/// A group name for this input when showing in a binding system
	/// </summary>
	[Group( "Display" )]
	public string GroupName { get; set; } = "Other";

	/// <summary>
	/// A friendly name for this input action when showing in a binding system
	/// </summary>
	[Group( "Display" )]
	public string Title { get; set; }

	/// <summary>
	/// The key or key combo we'll be watching for.
	/// </summary>
	[Editor( "keybind" ), Group( "Keybinds" )]
	public string KeyboardCode { get; set; }

	/// <summary>
	/// What gamepad button should this action map to?
	/// </summary>
	[JsonIgnore( Condition = JsonIgnoreCondition.Never ), Group( "Keybinds" )]
	public GamepadCode GamepadCode { get; set; } = GamepadCode.None;

	[return: MaybeNull]
	public IBlowoutInputKeyGroup FindKeyByName( string name )
	{
		if ( !IsEnabled )
			return default;

		if ( StringComparer.OrdinalIgnoreCase.Equals( name, KeyboardCode ) )
			return new BlowoutInputKey(KeyboardCode, BlowoutTeamSoft.Engine.LowLevel.BlowoutOperatingDevice.Windows, _actions);

		if ( StringComparer.OrdinalIgnoreCase.Equals( name, GamepadCode.ToString() ) )
			return new BlowoutInputKey( GamepadCode.ToString(), BlowoutTeamSoft.Engine.LowLevel.BlowoutOperatingDevice.XBox, _actions );

		return Keys.FirstOrDefault( x => StringComparer.OrdinalIgnoreCase.Equals( name, x.Name ) );
	}
}

public class BlowoutSource2InputCollection : IBlowoutInputActionCollection
{
	public InputAction Action { get; }

	public bool IsPressed
	{
		get
		{
			if ( !IsEnabled || !_IsRootActive() )
				return false;

			return Input.Pressed( Action.Name );
		}
	}

	public bool IsDown
	{
		get
		{
			if ( !IsEnabled || !_IsRootActive() )
				return false;

			return Input.Down( Action.Name );
		}
	}

	public bool IsReleased
	{
		get
		{
			if ( !IsEnabled || !_IsRootActive())
				return false;

			return Input.Released( Action.Name );
		}
	}

	public bool IsEnabled { get; set; } = true;

	public int Count => _binds.Actions.Count;

	public bool IsReadOnly => false;

	[IgnoreDataMember, JsonIgnore]
	private BindCollection _binds
	{
		get
		{
			if ( string.IsNullOrEmpty( Application.GameIdent ) )
				return null;

			field ??= InputBinds.FindCollection( Application.GameIdent );

			return field;
		}
	}

	[IgnoreDataMember, JsonIgnore]
	public BindCollection.ActionBind Bind => _binds.GetBind( Action.Name, false );

	public BlowoutSource2InputCollection(InputAction action )
	{
		Action = action;
	}

	private BindCollection.ActionBind _GetCurrentActionBind() =>
		_binds.GetBind( Action.Name );

	public void Add( IBlowoutInputKeyAction item )
	{
		if ( item is not InputAction inputAction )
			throw new BlowoutUnsupportedException( "It is not possible to set a non-native Input Action. Use the Input Action from the Source 2 Sdk engine, as it is installed as a Backend." );
		_binds.Set( Action.Name, item.Index, inputAction.KeyboardCode );
	}

	public void Clear()
	{
		_binds.Actions.Clear();
	}

	public bool Contains( IBlowoutInputKeyAction item )
	{
		if ( item is not InputAction inputAction )
			return false;
		return _GetCurrentActionBind().HasButton( inputAction.KeyboardCode );
	}

	private bool _IsRootActive()
	{
		if ( BlowoutEngine.Current == null )
			return false;

		return BlowoutEngine.Current.Input.IsActive;
	}

	public void CopyTo( IBlowoutInputKeyAction[] array, int arrayIndex )
	{
		throw new BlowoutUnsupportedException("Its not implemented. Its tooooo lazy.");
	}

	public IEnumerator<IBlowoutInputKeyAction> GetEnumerator()
	{
		throw new BlowoutUnsupportedException( "Its not implemented. Its tooooo lazy." );
	}

	public bool Remove( IBlowoutInputKeyAction item )
	{
		_binds.Set( Action.Name, item.Index, string.Empty);
		return true;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
