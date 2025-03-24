#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	using System;
	using System.IO;

	internal class FileFilter
	{
#pragma warning disable 0649
		public bool caseSensitive;
		public bool pathRecursive;
		public bool exactFileNameMatch;
		public bool exactPathMatch;

		public string filterPath;
		public string filterExtension;
		public string filterFileName;
#pragma warning restore 0649

		public bool MatchesPath(string filePath)
		{
			if (!caseSensitive)
				filePath = filePath.ToLowerInvariant();
			
			if (!string.IsNullOrWhiteSpace(filterExtension))
			{
				var extension = Path.GetExtension(filePath);
				if (string.IsNullOrEmpty(extension) || extension == ".")
					return false;

				extension = extension.Remove(0, 1);
				if (!filterExtension.Equals(extension,
					caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
					return false;
			}

			if (!string.IsNullOrWhiteSpace(filterFileName))
			{
				var fileName = Path.GetFileNameWithoutExtension(filePath);
				if (string.IsNullOrEmpty(fileName))
					return false;

				if (exactFileNameMatch)
				{
					if (!filterFileName.Equals(fileName, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
						return false;
				}
				else
				{
					if (fileName.IndexOf(filterFileName, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) == -1)
						return false;
				}
			}

			if (!string.IsNullOrWhiteSpace(filterPath))
			{
				if (string.IsNullOrWhiteSpace(filePath))
					return false;
				
				if (Path.DirectorySeparatorChar != Path.AltDirectorySeparatorChar &&
					filePath.IndexOf(Path.DirectorySeparatorChar) != -1)
					filePath = filePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				
				if (exactPathMatch)
				{
					if (!filePath.Equals(filterPath, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
						return false;
				}
				else
				{
					if (!PathMatch(filePath, filterPath, pathRecursive, !caseSensitive))
						return false;
				}
			}

			return true;
		}
		
		private static bool PathMatch(string filePath, string filter, bool includeNested, bool ignoreCase)
		{
			var index = filePath.IndexOf(filter,
				ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
			
			if (index == -1)
				return false;
			
			if (!includeNested)
			{
				var filterLength = filter.Length;
				var pathDelimiterIndex = filePath.IndexOf(Path.AltDirectorySeparatorChar, index + filterLength);
				if (pathDelimiterIndex != -1 && pathDelimiterIndex > index)
					return false;
			}

			return true;
		}

		public override string ToString()
		{
			return caseSensitive + "|" +
			       pathRecursive + "|" +
			       exactFileNameMatch + "|" +
			       exactPathMatch + "|" +
			       filterPath + "|" +
			       filterExtension + "|" +
			       filterFileName;
		}
	}
}