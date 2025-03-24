#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes.EditorCode;
using UnityEngine.SceneManagement;

namespace CodeStage.AntiCheat.EditorCode
{
	using Common;
	using ObscuredTypes;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;

	/// <summary>
	/// Class with utility functions to help with ACTk migrations after updates.
	/// </summary>
	public static class ObscuredTypesMigrator
	{
		private const string ModuleName = "Obscured Types Migration";
		private static bool fixOnlyMode = false;

		private static readonly string[] TypesToMigrate = 
		{
			nameof(ObscuredBigInteger),
			nameof(ObscuredBool),
			nameof(ObscuredDateTime),
			nameof(ObscuredDecimal),
			nameof(ObscuredDouble),
			nameof(ObscuredFloat),
			nameof(ObscuredInt),
			nameof(ObscuredLong),
			nameof(ObscuredQuaternion),
			nameof(ObscuredShort),
			nameof(ObscuredString),
			nameof(ObscuredUInt),
			nameof(ObscuredULong),
			nameof(ObscuredVector2),
			nameof(ObscuredVector2Int),
			nameof(ObscuredVector3),
			nameof(ObscuredVector3Int),
		};
		
		private delegate bool MigrateDelegate(SerializedProperty sp, bool fixOnly);
		
		private static readonly Dictionary<Type, MigrateDelegate> MigrateMappings = new Dictionary<Type, MigrateDelegate>
		{
			{ typeof(ObscuredBigInteger), Migrate<SerializedObscuredBigInteger> },
			{ typeof(ObscuredBool), Migrate<SerializedObscuredBool> },
			{ typeof(ObscuredDateTime), Migrate<SerializedObscuredDateTime> },
			{ typeof(ObscuredDecimal), Migrate<SerializedObscuredDecimal> },
			{ typeof(ObscuredDouble), Migrate<SerializedObscuredDouble> },
			{ typeof(ObscuredFloat), Migrate<SerializedObscuredFloat> },
			{ typeof(ObscuredInt), Migrate<SerializedObscuredInt> },
			{ typeof(ObscuredLong), Migrate<SerializedObscuredLong> },
			{ typeof(ObscuredQuaternion), Migrate<SerializedObscuredQuaternion> },
			{ typeof(ObscuredShort), Migrate<SerializedObscuredShort> },
			{ typeof(ObscuredString), Migrate<SerializedObscuredString> },
			{ typeof(ObscuredUInt), Migrate<SerializedObscuredUInt> },
			{ typeof(ObscuredULong), Migrate<SerializedObscuredULong> },
			{ typeof(ObscuredVector2), Migrate<SerializedObscuredVector2> },
			{ typeof(ObscuredVector2Int), Migrate<SerializedObscuredVector2Int> },
			{ typeof(ObscuredVector3), Migrate<SerializedObscuredVector3> },
			{ typeof(ObscuredVector3Int), Migrate<SerializedObscuredVector3Int> }
		};

		/// <summary>
		/// Checks all prefabs in project for old version of obscured types and tries to migrate values to the new version
		/// or fix corrupt states if possible.
		/// </summary>
		public static void MigrateProjectAssets(bool skipInteraction = false)
		{
			MigrateProjectAssets(false, skipInteraction);
		}

		/// <summary>
		/// Checks all prefabs in project for old version of obscured types and tries to migrate values to the new version
		/// or fix corrupt states if possible.
		/// </summary>
		public static void MigrateProjectAssets(bool fixOnly, bool skipInteraction)
		{
			if (!skipInteraction)
			{
				var result = EditorUtility.DisplayDialogComplex(ModuleName,
					"Are you sure you wish to scan all assets (except scenes) in your project and automatically migrate and / or fix invalid values?\n" +
					"You may need to migrate only if you did update from the older ACTk version.",
					"Migrate and fix", "Cancel", "Fix only");
				switch (result)
				{
					case 0:
						fixOnly = false;
						break;
					case 2:
						fixOnly = true;
						break;
					default:
						Debug.Log(ACTk.LogPrefix + ModuleName + ": canceled by user.");
						return;
				}
			}

			fixOnlyMode = fixOnly;
			EditorTools.TraverseSerializedScriptsAssets(ProcessProperty, TypesToMigrate);
		}

		/// <summary>
		/// Checks all currently opened scenes for old version of obscured types and tries to migrate values to the new version
		/// or fix corrupt states if possible.
		/// </summary>
		public static void MigrateOpenedScenes(bool skipInteraction = false)
		{
			MigrateOpenedScenes(false, skipInteraction);
		}

		/// <summary>
		/// Checks all currently opened scenes for old version of obscured types and tries to migrate values to the new version
		/// or fix corrupt states if possible.
		/// </summary>
		public static void MigrateOpenedScenes(bool fixOnly, bool skipInteraction, bool skipSave = false)
		{
			if (!skipInteraction)
			{
				var result = EditorUtility.DisplayDialogComplex(ModuleName,
					"Are you sure you wish to scan all opened scenes and automatically migrate and / or fix invalid values?\n" +
					"You may need to migrate only if you did update from the older ACTk version.",
					"Migrate and fix", "Cancel", "Fix only");
				switch (result)
				{
					case 0:
						fixOnly = false;
						break;
					case 2:
						fixOnly = true;
						break;
					default:
						Debug.Log(ACTk.LogPrefix + ModuleName + ": canceled by user.");
						return;
				}
			}
			
			fixOnlyMode = fixOnly;
			EditorTools.TraverseSerializedScriptsInScenes(ProcessProperty, TypesToMigrate, skipSave);
		}

		private static bool ProcessProperty(Object target, SerializedProperty sp, string label, string type)
		{
			var obscured = sp.GetValue<ISerializableObscuredType>();
            if (obscured == null || obscured.IsDataValid)
                return false;

            if (MigrateMappings.TryGetValue(obscured.GetType(), out var migrate))
            {
                var modified = migrate(sp, fixOnlyMode);

                if (modified)
                    Debug.Log($"{ACTk.LogPrefix}{ModuleName} migrated property {sp.displayName}:{type} at\n{label}", target);

                return modified;
            }

            return false;
		}
		
		private static bool Migrate<TSerialized>(SerializedProperty sp, bool fixOnly) where TSerialized : ISerializedObscuredType, new()
		{
			var serialized = new TSerialized();
			serialized.Init(sp);

			if (!fixOnly && serialized.IsCanMigrate)
				return serialized.Migrate();
			
			return serialized.Fix();
		}

		private static string GetWhatMigratesString(string[] typesToMigrate)
		{
			return string.Join(", ", typesToMigrate) + " will migrated.";
		}
	}
}