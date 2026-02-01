namespace Facepunch;

/// <summary>
/// macOS platform implementation
/// </summary>
internal class MacOSPlatform : Platform
{
	protected override string PlatformBaseName => "osx";

	public override bool CompileSolution( string solutionName, bool forceRebuild = false )
	{
		string buildArgs = forceRebuild ? "clean build" : "build";
		return Utility.RunProcess( "xcodebuild", $"-project {solutionName}.xcodeproj -configuration Blowout_Source2_Release {buildArgs}", "src" );
	}
}
