#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

#if UNITY_EDITOR

using CodeStage.AntiCheat.Utils;
using UnityEditor;

namespace CodeStage.AntiCheat.ObscuredTypes.EditorCode
{
	internal interface ISerializedObscuredType
	{
		bool IsCanMigrate { get; }
		SerializedProperty Target { get; }
		void Init(SerializedProperty target);
		bool Fix();
		bool Migrate();
		string GetMigrationResultString();
	}

	internal abstract class MigratableSerializedObscuredType<T> : SerializedObscuredType<T>
	{
		public override bool IsCanMigrate => true;

		public abstract override string GetMigrationResultString();
		
		public sealed override bool Migrate()
		{
			if (IsDataValid) // valid, no need to migrate
				return false;

			var migrated = PerformMigrate();
			if (migrated)
			{
				Target.serializedObject.ApplyModifiedProperties();
				Fix();
			}
			
			return migrated;
		}
		
		protected abstract bool PerformMigrate();
	}
	
	public abstract class SerializedObscuredType<T> : ISerializedObscuredType
	{
		internal const string ObsoleteMigrationVersion = "2";
		
		public virtual bool IsCanMigrate => false;
		public abstract bool IsDataValid { get; }
		public abstract T Plain { get; }
		public SerializedProperty Target { get; private set; }
		
		internal SerializedProperty HashProperty { get; private set; }
		internal SerializedProperty HiddenProperty { get; private set; }
		internal SerializedProperty KeyProperty { get; private set; }
		internal SerializedProperty VersionProperty { get; private set; }
		
		protected virtual string HiddenPropertyRelativePath => nameof(ObscuredInt.hiddenValue);
		protected virtual string KeyPropertyRelativePath => nameof(ObscuredInt.currentCryptoKey);
		protected abstract byte TypeVersion { get; }
		
		public int Hash
		{
			get => HashProperty.intValue;
			set => HashProperty.intValue = value;
		}
		
		public int Version
		{
			get => VersionProperty.intValue;
			set => VersionProperty.intValue = value;
		}
		
		public SerializedObscuredType()
		{
			
		}
		
		public SerializedObscuredType(SerializedProperty sp)
		{
			Init(sp);
		}

		public void Init(SerializedProperty target)
		{
			Target = target;
			HashProperty = target.FindPropertyRelative("hash");
			HiddenProperty = target.FindPropertyRelative(HiddenPropertyRelativePath);
			KeyProperty = target.FindPropertyRelative(KeyPropertyRelativePath);
			VersionProperty = target.FindPropertyRelative("version");
		}

		public virtual bool Fix()
		{
			if (IsDataValid)
				return false;

			var validHash = HashUtils.CalculateHashGeneric(Plain);
			Hash = validHash;
			Version = TypeVersion;
			Target.serializedObject.ApplyModifiedProperties();
			return true;
		}
		
		public virtual bool Migrate()
		{
			throw new System.NotImplementedException();
		}

		public virtual string GetMigrationResultString()
		{
			throw new System.NotImplementedException();
		}
	}
}

#endif