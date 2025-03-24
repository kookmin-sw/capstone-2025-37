#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.EditorCode
{
	using System;
	using Common;
	using UnityEditor;
	using UnityEngine;
	using Object = UnityEngine.Object;
#if UNITY_2021_2_OR_NEWER
	using UnityEditor.Build;
#endif

	internal static class SettingsUtils
	{
		private static SymbolsData cachedSymbolsData;
		
		public static bool IsIL2CPPEnabled()
		{
			// ReSharper disable once ConvertIfStatementToReturnStatement - it's not redundant ;)
			if (IsScriptingBackendIL2CPP())
				return true;

#if ENABLE_IL2CPP
			return true;
#else
			return false;
#endif
			
			bool IsScriptingBackendIL2CPP()
			{
#if UNITY_2021_2_OR_NEWER
				var namedTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
				var scriptingBackend = PlayerSettings.GetScriptingBackend(namedTarget);
#else
				var scriptingBackend = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
#endif
				return scriptingBackend == ScriptingImplementation.IL2CPP;
			}
		}
		
		public static bool IsIL2CPPSupported()
		{
			return IsIL2CPPEnabled() || ReflectionTools.IsScriptingImplementationSupported(ScriptingImplementation.IL2CPP, EditorUserBuildSettings.selectedBuildTargetGroup);
		}

		public static bool IsLinkXmlRequired()
		{
			return IsIL2CPPEnabled() && PlayerSettings.stripEngineCode;
		}
		
		public static bool IsLinkXmlEnabled()
		{
			return GetSymbolsData().wallhackLinkXML;
		}
		
		public static SymbolsData GetSymbolsData()
		{
			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			foreach (var buildTargetGroup in groups)
			{
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;
				if (IsBuildTargetGroupNameObsolete(buildTargetGroup.ToString())) continue;

				try
				{
					var symbols = GetConditionalSymbolsForPlatform(buildTargetGroup);
					
					cachedSymbolsData.wallhackLinkXML |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.WallhackLinkXML);
					cachedSymbolsData.excludeObfuscation |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.ExcludeObfuscation);
					cachedSymbolsData.preventReadPhoneState |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.PreventReadPhoneState);
					cachedSymbolsData.preventInternetPermission |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.PreventInternetPermission);
					cachedSymbolsData.exposeThirdPartyIntegration |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.ThirdPartyIntegration);
					cachedSymbolsData.usExportCompatible |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.UsExportCompatible);
					cachedSymbolsData.newtonsoftJson |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.NewtonsoftJson);

					cachedSymbolsData.injectionDebug |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.InjectionDebug);
					cachedSymbolsData.injectionDebugVerbose |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.InjectionDebugVerbose);
					cachedSymbolsData.injectionDebugParanoid |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.InjectionDebugParanoid);
					cachedSymbolsData.wallhackDebug |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.WallhackDebug);
					cachedSymbolsData.detectionBacklogs |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.DetectionBacklogs);
					cachedSymbolsData.genericDevLogs |= GetSymbol(symbols, ACTkEditorConstants.Conditionals.GenericDevLogs);
				}
				catch (Exception)
				{
					// ignoring obsolete ones
				}
			}

			return cachedSymbolsData;
		}

		private static string GetConditionalSymbolsForPlatform(BuildTargetGroup buildTargetGroup)
		{
#if UNITY_2021_2_OR_NEWER
			var namedTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
			return PlayerSettings.GetScriptingDefineSymbols(namedTarget);
#else
			return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
#endif
		}

		private static bool GetSymbol(string symbols, string symbol)
		{
			var result = false;

			if (symbols == symbol)
				result = true;
			else if (symbols.StartsWith(symbol + ';'))
				result = true;
			else if (symbols.EndsWith(';' + symbol))
				result = true;
			else if (symbols.Contains(';' + symbol + ';'))
				result = true;

			return result;
		}

		public static void SwitchSymbol(string symbol, bool active)
		{
			var groups = (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup));
			
			// this is required to get names for obsolete items alternatives like iOS instead of deprecated iPhone
			var names = Enum.GetNames(typeof(BuildTargetGroup));
			
			for (var i = 0; i < groups.Length; i++)
			{
				var buildTargetGroup = groups[i];
				var buildTargetName = names[i];
				
				if (buildTargetGroup == BuildTargetGroup.Unknown) continue;
				if (IsBuildTargetGroupNameObsolete(buildTargetName)) continue;

				try
				{
					var symbols = GetConditionalSymbolsForPlatform(buildTargetGroup);

					if (active)
						AddSymbol(ref symbols, symbol);
					else
						RemoveSymbol(ref symbols, symbol);

#if UNITY_2021_2_OR_NEWER
					var namedTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
					PlayerSettings.SetScriptingDefineSymbols(namedTarget, symbols);
#else
					PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
#endif
				}
				catch (Exception)
				{
					// ignoring obsolete ones
				}
			}
		}

		private static void AddSymbol(ref string symbols, string symbol)
		{
			if (symbols.Length == 0)
			{
				symbols = symbol;
			}
			else
			{
				if (symbols.EndsWith(";"))
					symbols += symbol;
				else
					symbols += ';' + symbol;
			}
		}

		private static void RemoveSymbol(ref string symbols, string symbol)
		{
			if (symbols == symbol)
				symbols = string.Empty;
			else if (symbols.StartsWith(symbol + ';'))
				symbols = symbols.Remove(0, symbol.Length + 1);
			else if (symbols.EndsWith(';' + symbol))
				symbols = symbols.Remove(symbols.LastIndexOf(';' + symbol, StringComparison.Ordinal), symbol.Length + 1);
			else if (symbols.Contains(';' + symbol + ';'))
				symbols = symbols.Replace(';' + symbol + ';', ";");
		}

		private static bool IsBuildTargetGroupNameObsolete(string name)
		{
			var fi = typeof(BuildTargetGroup).GetField(name);
			var attributes = (ObsoleteAttribute[])fi.GetCustomAttributes(typeof(ObsoleteAttribute), false);
			return attributes.Length > 0;
		}

		public static bool IsPhysicsUsesTemporalSolver()
		{
			var solverTypeProperty = GetPhysicsSolverTypeProperty();
			if (solverTypeProperty == null)
				return false;

			return solverTypeProperty.intValue == 1;
		}

		private static Object LoadDynamicManager()
		{
			var dynamicsManager = AssetDatabase.LoadAssetAtPath<Object>("ProjectSettings/DynamicsManager.asset");
			if (dynamicsManager == null)
				ACTk.PrintExceptionForSupport("Couldn't find the DynamicsManager");
			
			return dynamicsManager;
		}
		
		private static SerializedProperty GetPhysicsSolverTypeProperty()
		{
			var dynamicsManager = LoadDynamicManager();
			if (dynamicsManager == null)
				return null;
			
			var so = new SerializedObject(dynamicsManager);
			var field = so.FindProperty("m_SolverType");

			if (field == null)
				ACTk.PrintExceptionForSupport("Couldn't find the m_SolverType field in the DynamicsManager class!");
			
			return field;
		}

		public static void ShowPhysicsSettings()
		{
			SettingsService.OpenProjectSettings("Project/Physics");
		}

		public static void ShowSolverTypesProperty()
		{
			ShowPhysicsSettings();

#if UNITY_2021_3_OR_NEWER
			EditorApplication.delayCall += () =>
			{
				var solverTypeProperty = GetPhysicsSolverTypeProperty();
				if (solverTypeProperty != null)
					Highlighter.Highlight("Project Settings", solverTypeProperty.propertyPath, HighlightSearchMode.Identifier);
			};
#endif
		}
	}
}