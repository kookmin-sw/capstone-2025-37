namespace CodeStage.AntiCheat.EditorCode
{
	using System.Linq;
	using Common;
	using ObscuredTypes;
	using UnityEditor;
	using UnityEngine;

	public delegate void InvalidPropertyFound(Object target, SerializedProperty sp, string location, string type);
	
	public static class ObscuredTypesValidator
	{
		internal const string ModuleName = "Obscured Types Validator";
		
		private static InvalidPropertyFound lastPassedCallback;
		private static int invalidPropertiesFound;
		
		/// <summary>
		/// Traverses all prefabs and scriptable objects in the project and checks if they contain any Obscured types with anomalies.
		/// </summary>
		/// <param name="callback">Pass callback if you wish to process invalid objects.</param>
		/// <param name="skipInteraction">Skips intro confirmation.</param>
		/// <returns>Number of invalid properties.</returns>
		public static int ValidateProjectAssets(InvalidPropertyFound callback = null, bool skipInteraction = false)
		{
			invalidPropertiesFound = 0;
			
			if (!EditorUtility.DisplayDialog(ModuleName,
					"Are you sure you wish to scan all assets (except scenes) in your project and validate all found obscured types?\n" +
					"This can take some time to complete.",
					"Yes", "No"))
			{
				Debug.Log(ACTk.LogPrefix + ModuleName + ": canceled by user.");
				return invalidPropertiesFound;
			}

			var types = TypeCache.GetTypesDerivedFrom<ISerializableObscuredType>().Where(t => !t.IsAbstract && !t.IsInterface)
				.Select(type => type.Name).ToArray();
			lastPassedCallback = callback;
			EditorTools.TraverseSerializedScriptsAssets(ValidateProperty, types);
			lastPassedCallback = null;
			
			Debug.Log(ACTk.LogPrefix + ModuleName + ": complete.");

			return invalidPropertiesFound;
		}

		/// <summary>
		/// Traverse all currently opened scenes and checks if they contain any Components with non-valid Obscured types.
		/// </summary>
		/// <param name="callback">Pass callback if you wish to process invalid objects.</param>
		/// <param name="skipInteraction">Skips intro confirmation.</param>
		/// <returns>Number of invalid properties.</returns>
		public static int ValidateOpenedScenes(InvalidPropertyFound callback = null, bool skipInteraction = false)
		{
			invalidPropertiesFound = 0;
			
			if (!skipInteraction && !EditorUtility.DisplayDialog(ModuleName,
				    "Are you sure you wish to scan all opened Scenes and validate all found obscured types?\n" +
				    "This can take some time to complete.",
				    "Yes", "No"))
			{
				Debug.Log(ACTk.LogPrefix + ModuleName + ": canceled by user.");
				return invalidPropertiesFound;
			}
			
			var types = TypeCache.GetTypesDerivedFrom<ISerializableObscuredType>().Where(t => !t.IsAbstract && !t.IsInterface)
				.Select(type => type.Name).ToArray();
			lastPassedCallback = callback;
			EditorTools.TraverseSerializedScriptsInScenes(ValidateProperty, types);
			lastPassedCallback = null;
			
			Debug.Log(ACTk.LogPrefix + ModuleName + ": complete.");

			return invalidPropertiesFound;
		}
		
		private static bool ValidateProperty(Object target, SerializedProperty sp, string location, string type)
		{
			var obscured = sp.GetValue<ISerializableObscuredType>();
			if (!obscured.IsDataValid)
			{
				lastPassedCallback?.Invoke(target, sp, location, type);
				target = EditorTools.GetPingableObject(target);
				Debug.LogWarning($"{ACTk.LogPrefix}{ModuleName} found invalid property [{sp.displayName}] of type [{type}] at:\n{location}", target);
				invalidPropertiesFound++;
			}

			return false;
		}
	}
}