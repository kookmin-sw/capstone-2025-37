#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	internal struct FilesProgress
	{
#if UNITY_EDITOR
		private float progress;
		private string fileName;
#endif

		public static FilesProgress Step(float progress, string fileName)
		{
			return new FilesProgress
			{
#if UNITY_EDITOR
				progress = progress,
				fileName = fileName
#endif
			};
		}
		
		public static FilesProgress None()
		{
			return new FilesProgress
			{
#if UNITY_EDITOR
				progress = -1,
				fileName = null
#endif
			};
		}

		public static IProgress<FilesProgress> CreateNew(string header)
		{
			return new Progress<FilesProgress>(value =>
			{
#if UNITY_EDITOR
				if (value.progress >= 0)
					UnityEditor.EditorUtility.DisplayProgressBar($"{header} {value.progress}%", $"{value.fileName} done",
						value.progress);
				else
					UnityEditor.EditorUtility.ClearProgressBar();
#endif
				
			});
		}
	}
}