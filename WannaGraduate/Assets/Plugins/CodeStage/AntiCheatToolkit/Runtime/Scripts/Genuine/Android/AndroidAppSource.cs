#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Genuine.Android
{
	/// <summary>
	/// Guessed source of the app installation.
	/// </summary>
	public enum AndroidAppSource
	{
		/// <summary>
		/// App was installed on-device from the apk with the package installer.
		/// </summary>
		PackageInstaller,
		GooglePlayStore,
		AmazonAppStore,
		HuaweiAppGallery,
		SamsungGalaxyStore,
		/// <summary>
		/// Source of the app installation is unknown.
		/// </summary>
		Other,
		/// <summary>
		/// There was a problem accessing the app installation source information.
		/// </summary>
		AccessError
	}
}