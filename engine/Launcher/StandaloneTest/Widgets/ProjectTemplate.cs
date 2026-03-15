using BlowoutTeamSoft.Engine;
using BlowoutTeamSoft.Source2.Core;
using NLog;
using Sandbox.DataModel;
using System.IO;
using System.Text.Json;
namespace Editor;

internal class ProjectTemplate
{
	ProjectConfig Config { get; init; }
	string TemplatePath { get; init; }

	public string AddonType => Config.Type.ToLower();
	public string Title => Config.Title;

	public string Icon { get; set; } = "question_mark";
	public int Order { get; set; } = 0;
	public string Description { get; set; } = "No description provided.";

	public ProjectTemplate( ProjectConfig templateConfig, string path )
	{
		Config = templateConfig;
		TemplatePath = path;

		if ( Config.TryGetMeta( "ProjectTemplate", out DisplayData display ) )
		{
			Icon = display.Icon;
			Order = display.Order ?? 0;
			Description = display.Description ?? "No description provided.";
		}
	}

	public void Apply( string targetDir, ref ProjectConfig config )
	{
		var sourceDir = Editor.FileSystem.Root.GetFullPath( TemplatePath );

		if ( !Directory.Exists( sourceDir ) )
			return;

		//Log.Info( $"Copying {Title} template to destination folder: {targetDir}" );

		CopyFolder( sourceDir, targetDir, config.Ident, config.Title );

		// Grab the .sbproj from the template, and apply what we can.
		// We'll store the old ident and title and restore it later
		var ident = config.Ident;
		var title = config.Title;

		var fullPath = Path.Combine( sourceDir, "$ident.sbproj" );
		var bxprojPath = Path.Combine( sourceDir, "$ident.bxproj" );

		if ( File.Exists( fullPath ) )
		{
			config = JsonSerializer.Deserialize<ProjectConfig>( File.ReadAllText( fullPath ) );

			// Restore ident & title from our addon's preferences
			config.Ident = ident;
			config.Title = title;

			config.SetMeta( "ProjectTemplate", null );

			//Log.Info( $"OK" );
		}

		if(File.Exists(bxprojPath))
		{
			config = (ProjectConfig)BlowoutSource2Engine.BSource2XProject.ReadFromFile( bxprojPath );

			config.Ident = ident;
			config.Title = title;

			config.SetMeta( "ProjectTemplate", null );
			//Log.Warning( $"Something went wrong while applying template to new project" );
		}
	}

	void CopyFolder( string sourceDir, string targetDir, string addonIdent, string title )
	{
		if ( sourceDir.Contains( "\\.", StringComparison.OrdinalIgnoreCase ) )
		{
			return;
		}

		Directory.CreateDirectory( targetDir );

		foreach ( var file in Directory.GetFiles( sourceDir ) )
		{
			CopyAndProcessFile( file, targetDir, addonIdent, title );
		}

		foreach ( var directory in Directory.GetDirectories( sourceDir ) )
		{
			CopyFolder( directory, Path.Combine( targetDir, Path.GetFileName( directory ) ), addonIdent, title );
		}
	}

	void CopyAndProcessFile( string file, string targetDir, string addonIdent, string title )
	{
		var targetname = Path.Combine( targetDir, Path.GetFileName( file ) );

		// Replace $ident with our ident in file name
		targetname = targetname.Replace( "$ident", addonIdent );

		if ( file.EndsWith( ".cs" ) || file.EndsWith( ".json" ) )
		{
			var txt = System.IO.File.ReadAllText( file );
			txt = txt.Replace( "$title", title );
			txt = txt.Replace( "$ident", addonIdent );
			System.IO.File.WriteAllText( targetname, txt );
		}
		else
		{
			if ( BlowoutSource2Engine.BSource2XProject.IsLegacy( targetname ) )
			{
				var proj = (ProjectConfig)BlowoutSource2Engine.BSource2XProject.ConvertFromLegacy( File.ReadAllText(file) );
				File.WriteAllText(targetname, proj.ToXml());
			}
			else
				File.Copy( file, targetname );
		}
	}

	class DisplayData
	{
		public string Icon { get; set; }
		public int? Order { get; set; }
		public string Description { get; set; }
	}
}
