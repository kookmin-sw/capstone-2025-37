#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using CodeStage.AntiCheat.Genuine.CodeHash;

namespace CodeStage.AntiCheat.Utils
{
	internal static class ProgressExtensionMethods
	{
		public static void ReportPercent(this IProgress<FilesProgress> progress, ref int latestPercent, int filesChecked, long count, string currentFile)
		{
			if (progress == null)
				return;

			var progressPercent = (float)filesChecked / count;
			var intPercent = (int)Math.Floor(progressPercent * 100);
			if (latestPercent < intPercent)
			{
				latestPercent = intPercent;
				progress.Report(FilesProgress.Step(latestPercent, currentFile));
			}
		}
	}
}