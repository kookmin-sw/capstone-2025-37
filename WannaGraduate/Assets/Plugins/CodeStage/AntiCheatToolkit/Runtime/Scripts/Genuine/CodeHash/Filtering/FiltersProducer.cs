#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeStage.AntiCheat.Genuine.CodeHash
{
	internal static class FiltersProducer
	{
		private static FilteringSettings LoadSettings()
		{
			return ScriptableObject.CreateInstance<FilteringSettings>();
			/*return Resources.Load<FilteringSettings>("net.codestage.actk/HashFilteringSettings");*/
		}

		public static FilteringData GetFileFiltersAndroid(bool il2Cpp)
		{
			var group = LoadSettings().DefaultGroup;
			switch (group)
			{
				case FilterGroup.Code:
					return GetCodeFiltersForAndroid(il2Cpp);
				case FilterGroup.Content:
					return GetContentFiltersForAndroid(il2Cpp);
				case FilterGroup.All:
					return GetAllFiltersForAndroid(il2Cpp);
				default:
					throw new ArgumentOutOfRangeException(nameof(group), group, null);
			}
		}
		
		public static FilteringData GetFileFiltersStandaloneWindows(bool il2Cpp)
		{
			var group = LoadSettings().DefaultGroup;
			switch (group)
			{
				case FilterGroup.Code:
					return GetCodeFiltersForWindows(il2Cpp);
				case FilterGroup.Content:
					return GetContentFiltersForWindows(il2Cpp);
				case FilterGroup.All:
					return GetAllFiltersForWindows(il2Cpp);
				default:
					throw new ArgumentOutOfRangeException(nameof(group), group, null);
			}
		}

		private static FilteringData GetCodeFiltersForAndroid(bool il2Cpp)
		{
			var includes = new List<FileFilter>
			{
				new FileFilter
				{
					filterExtension = "dex"
				},
				new FileFilter
				{
					filterExtension = "so"
				}
			};
        
			if (!il2Cpp)
			{
				includes.Add(new FileFilter
				{
					filterExtension = "dll"
				});
			}
			else
			{
				includes.Add(new FileFilter
				{
					filterFileName = "global-metadata",
					filterExtension = "dat"
				});
			}
        
			return new FilteringData(includes.ToArray());
		}

		private static FilteringData GetContentFiltersForAndroid(bool il2Cpp)
		{
			var includes = new List<FileFilter>
			{
				new FileFilter
				{
					filterPath = "Resources/unity_builtin_extra",
				},
				new FileFilter
				{
					filterPath = "bin/Data/",
					pathRecursive = false,
					caseSensitive = true
				}
			};

			return new FilteringData(includes.ToArray());
		}

		private static FilteringData GetAllFiltersForAndroid(bool il2Cpp)
		{
			var codeFilters = GetCodeFiltersForAndroid(il2Cpp);
			var contentFilters = GetContentFiltersForAndroid(il2Cpp);
			return codeFilters.Join(contentFilters);
		}
		
		private static FilteringData GetCodeFiltersForWindows(bool il2Cpp)
		{
			var includes = new List<FileFilter>
			{
				new FileFilter
				{
					filterExtension = "dll"
				},
				new FileFilter
				{
					filterExtension = "exe"
				}
			};

			FileFilter[] ignores;

			if (il2Cpp)
			{
				includes.Add(new FileFilter
				{
					filterFileName = "global-metadata",
					filterExtension = "dat",
				});
		        
				ignores = new[]
				{
					new FileFilter
					{
						filterPath = "_BackUpThisFolder_ButDontShipItWithYourGame",
						caseSensitive = true,
						exactPathMatch = false,
						pathRecursive = true,
					}
				};
			}
			else
			{
				ignores = null;
			}

			return new FilteringData(includes.ToArray(), ignores);
		}

		private static FilteringData GetContentFiltersForWindows(bool il2Cpp)
		{
			var includes = new List<FileFilter>
			{
				new FileFilter
		        {
			        filterPath = "_Data/",
					pathRecursive = false,
			        caseSensitive = true,
			        exactPathMatch = false,
				},
				new FileFilter
				{
					filterPath = "Resources/unity default resources",
				},
				new FileFilter
				{
					filterPath = "Resources/unity_builtin_extra",
				},
			};
			
			return new FilteringData(includes.ToArray());
		}

		private static FilteringData GetAllFiltersForWindows(bool il2Cpp)
		{
			var codeFilters = GetCodeFiltersForWindows(il2Cpp);
			var contentFilters = GetContentFiltersForWindows(il2Cpp);
			return codeFilters.Join(contentFilters);
		}
	}
}