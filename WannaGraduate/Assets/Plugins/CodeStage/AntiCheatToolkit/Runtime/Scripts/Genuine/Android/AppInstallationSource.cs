#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Genuine.Android
{
	/// <summary>
	/// Holds information about the app installation source.
	/// </summary>
	public class AppInstallationSource
	{
		/// <summary>
		///	Package name of the installation source, for example "com.android.vending" for Google Play Store.
		/// </summary>
		public string PackageName { get; }
		
		/// <summary>
		/// Detected source of the app installation to simplify further processing.
		/// </summary>
		public AndroidAppSource DetectedSource { get; }
		
		internal AppInstallationSource(string packageName)
		{
			PackageName = packageName;
			DetectedSource = GetStoreName(packageName);
		}
		
		private AndroidAppSource GetStoreName(string packageName)
		{
			if (packageName == null)
				return AndroidAppSource.AccessError;
				
			switch (packageName)
			{
				case "com.android.vending":
					return AndroidAppSource.GooglePlayStore;
				case "com.amazon.venezia":
					return AndroidAppSource.AmazonAppStore;
				case "com.huawei.appmarket":
					return AndroidAppSource.HuaweiAppGallery;
				case "com.sec.android.app.samsungapps":
					return AndroidAppSource.SamsungGalaxyStore;
				case "com.google.android.packageinstaller":
				case "com.android.packageinstaller":
					return AndroidAppSource.PackageInstaller;
				default:
					return AndroidAppSource.Other;
			}
		}
	}
}