using BlowoutTeamSoft.Engine.Assets;

namespace Sandbox;

public partial class Resource
{
	/// <summary>
	/// Get the icon for this type of asset. This is an icon that is shown in the editor.
	/// </summary>
	public Bitmap GetAssetTypeIcon( int width, int height )
	{
		// cache opportunity at some point
		return CreateAssetTypeIcon( width, height );
	}

	/// <summary>
	/// Create an icon for this type of asset. This is an icon that is shown in the editor.
	/// </summary>
	protected virtual Bitmap CreateAssetTypeIcon( int width, int height )
	{
		// backwards compatibility
#pragma warning disable CS0618
		var type = this.GetType();
		var gr = Sandbox.Game.TypeLibrary.GetAttribute<GameResourceAttribute>(type);
		var blowoutInstanceAttribute = Sandbox.Game.TypeLibrary.GetAttribute<BlowoutAssetInstanceIconAttribute>(type);

		var icon = blowoutInstanceAttribute?.IconName ?? gr?.Icon ?? "question_mark";
		Color? foreground = null;
		Color? background = null;

		if (blowoutInstanceAttribute != null && !string.IsNullOrEmpty(blowoutInstanceAttribute.ForegroundColorHex))
			foreground = blowoutInstanceAttribute.ForegroundColorHex;

		if (blowoutInstanceAttribute != null && !string.IsNullOrEmpty(blowoutInstanceAttribute.BackgroundColorHex))
			background = blowoutInstanceAttribute.BackgroundColorHex;

		Color fg = foreground ?? gr?.IconFgColor ?? "#1a2c17";
		Color bg = background ?? gr?.IconBgColor ?? "#67ac5c";

		if (string.IsNullOrEmpty(icon)) icon = "question_mark";

		// we're not supporting loading from paths anymore. I don't think we ever did?
		if (icon.Contains("/") || icon.Contains("\\")) icon = "question_mark";

		return CreateSimpleAssetTypeIcon(icon, width, height, bg, fg);
#pragma warning restore CS0618
	}

	/// <summary>
	/// Create a simple icon using an icon. This is used by default for asset types.
	/// </summary>
	protected static Bitmap CreateSimpleAssetTypeIcon( string icon, int width, int height, Color? background = default, Color? foreground = default )
	{
		Color fg = foreground ?? "#1a2c17";
		Color bg = background ?? "#67ac5c";

		var bitmap = new Bitmap( width, height );
		bitmap.Clear( new Color( 0, 0, 0, 0 ) );

		bitmap.SetRadialGradient( width * 0.1f, height * 2, Gradient.FromColors( bg, bg.Darken( 0.25f ) ) );
		bitmap.DrawRoundRect( bitmap.Rect, 4 );

		bitmap.DrawText( new TextRendering.Scope( icon, fg, height * 0.8f, "Material Icons" ), bitmap.Rect, TextFlag.Center | TextFlag.DontClip );
		return bitmap;
	}
}
