#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using UnityEngine;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	// [CreateAssetMenu(menuName = "Code Stage/Anti-Cheat Toolkit/Code Hash/Filtering Settings")]
	// Going to keep this in ProjectSettings and export to encrypted json on build + include it to the hashing checks
	internal class FilteringSettings : ScriptableObject
	{
		[field:SerializeField]
		public FilterGroup DefaultGroup { get; private set; } = FilterGroup.Code;
		/*public FileFilter[] CustomIncludes { get; }
		public FileFilter[] CustomIgnores { get; }*/
	}
}