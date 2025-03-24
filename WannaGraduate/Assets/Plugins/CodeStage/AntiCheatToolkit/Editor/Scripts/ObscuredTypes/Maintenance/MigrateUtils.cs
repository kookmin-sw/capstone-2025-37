#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.EditorCode
{
	public static class MigrateUtils
	{
		[Obsolete("Use " + nameof(ObscuredTypesMigrator) + "." + nameof(ObscuredTypesMigrator.MigrateProjectAssets)  + "(). " +
				  "This method will be removed in future versions.", true)]
		public static void MigrateObscuredTypesInAssets(bool skipInteraction = false)
		{
			ObscuredTypesMigrator.MigrateProjectAssets(skipInteraction);
		}

		[Obsolete("Use " + nameof(ObscuredTypesMigrator) + "." + nameof(ObscuredTypesMigrator.MigrateProjectAssets)  + "(). " +
				  "This method will be removed in future versions.", true)]
		public static void MigrateObscuredTypesInAssets(bool fixOnly, bool skipInteraction)
		{
			ObscuredTypesMigrator.MigrateProjectAssets(fixOnly, skipInteraction);
		}

		[Obsolete("Use " + nameof(ObscuredTypesMigrator) + "." + nameof(ObscuredTypesMigrator.MigrateOpenedScenes)  + "(). " +
				  "This method will be removed in future versions.", true)]
		public static void MigrateObscuredTypesInScene(bool skipInteraction = false)
		{
			ObscuredTypesMigrator.MigrateOpenedScenes(skipInteraction);
		}

		[Obsolete("Use " + nameof(ObscuredTypesMigrator) + "." + nameof(ObscuredTypesMigrator.MigrateOpenedScenes)  + "(). " +
				  "This method will be removed in future versions.", true)]
		public static void MigrateObscuredTypesInScene(bool fixOnly, bool skipInteraction, bool skipSave = false)
		{
			ObscuredTypesMigrator.MigrateOpenedScenes(fixOnly, skipInteraction, skipSave);
		}
	}
}