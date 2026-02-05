namespace Editor;


class LogoWidget : Widget
{
	public LogoWidget( Widget parent ) : base( parent )
	{
	}

	protected override Vector2 SizeHint()
	{
		return new Vector2( 32 );
	}

	protected override void OnPaint()
	{
		Paint.ClearPen();
		Paint.ClearBrush();
		Paint.SetDefaultFont();

		var r = LocalRect;
		r.Width = r.Height;

		Paint.Draw( r, "blow_logo_rounded.png" );
	}
}
