#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
    internal class FilteringData
    {
		public FileFilter[] Includes { get; } // used from Android-specific code
		public FileFilter[] Ignores { get; }

		public FilteringData(FileFilter[] includes, FileFilter[] ignores = null)
        {
            Includes = includes;
            Ignores = ignores;
        }
        
        public bool IsIncluded(string path)
        {
            return IsPathMatchesFilters(path, Includes);
        }

        public bool IsIgnored(string path)
        {
	        if (Ignores == null || Ignores.Length == 0)
		        return false;
	        
            return IsPathMatchesFilters(path, Ignores);
        }
		
		public FilteringData Join(FilteringData contentFilters)
		{
			return new FilteringData(
				JoinArrays(Includes, contentFilters.Includes),
				JoinArrays(Ignores, contentFilters.Ignores));
		}

        private bool IsPathMatchesFilters(string path, FileFilter[] filters)
        {
            foreach (var filter in filters)
            {
                if (filter.MatchesPath(path))
                    return true;
            }

            return false;
        }
		
		private T[] JoinArrays<T>(T[] a1, T[] a2)
		{
			if (a1 == null && a2 == null) return Array.Empty<T>();
			if (a1 == null) return a2;
			if (a2 == null) return a1;

			var newArray = new T[a1.Length + a2.Length];
			Array.Copy(a1, 0, newArray, 0, a1.Length);
			Array.Copy(a2, 0, newArray, a1.Length, a2.Length);
			return newArray;
		}
    }
}