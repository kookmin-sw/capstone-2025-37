#region copyright
// ------------------------------------------------------
// Copyright (C) Dmitriy Yukhanov [https://codestage.net]
// ------------------------------------------------------
#endregion

namespace CodeStage.AntiCheat.Detectors
{
	using Common;

	using System;
	using ObscuredTypes;
	using UnityEngine;

	/// <summary>
	/// Contains detailed information about latest Obscured Types cheating detection.
	/// </summary>
	public class ObscuredCheatingDetectionInfo
	{
		/// <summary>
		/// Type of the source. Holds type of the obscured type instance which triggered the detection.
		/// </summary>
		public Type SourceType { get; }
		
		/// <summary>
		/// Indicates encrypted value passed hash validation and is genuine.
		/// </summary>
		public bool HashValid { get; }
		
		/// <summary>
		/// Actual encrypted value (in clean decrypted form) at the detection moment.
		/// </summary>
		/// Please note, some types have both whole values and separate components checks,
		/// for example, ObscuredVector3 has checks for whole Vector3 and its components like Vector3.x,
		/// thus this value can hold either the whole struct or just one of its components.
		public object ObscuredValue { get; }
		
		/// <summary>
		/// Faked "honeypot" value at the detection moment (if honeyPot option is enabled).
		/// </summary>
		/// Please note, some types have both whole values and separate components checks,
		/// for example, ObscuredVector3 has checks for whole Vector3 and its components like Vector3.x,
		/// thus this value can hold either the whole struct or just one of its components.
		public object FakeValue { get; }

		public ObscuredCheatingDetectionInfo(Type type, bool hashValid, object decrypted, object fake)
		{
			SourceType = type;
			HashValid = hashValid;
			ObscuredValue = decrypted;
			FakeValue = fake;
		}

		public override string ToString()
		{
			return $"Type: {SourceType}\n" +
				   $"Hash Valid: {HashValid}\n" +
				   $"Decrypted: {ObscuredValue}\n" +
				   $"Fake: {FakeValue}";
		}
	}

	/// <summary>
	/// Detects CodeStage.AntiCheat.ObscuredTypes cheating.
	/// </summary>
	/// It allows cheaters to find desired (fake) values in memory and change them, keeping original values secure.<br/>
	/// It's like a cheese in the mouse trap - cheater tries to change some obscured value and get caught on it.
	///
	/// Just add it to any GameObject as usual or through the "GameObject > Create Other > Code Stage > Anti-Cheat Toolkit"
	/// menu to get started.<br/>
	/// You can use detector completely from inspector without writing any code except the actual reaction on cheating.
	///
	/// Avoid using detectors from code at the Awake phase.
	[AddComponentMenu(MenuPath + ComponentName)]
	[DisallowMultipleComponent]
	[HelpURL(ACTk.DocsRootUrl + "class_code_stage_1_1_anti_cheat_1_1_detectors_1_1_obscured_cheating_detector.html")]
	public class ObscuredCheatingDetector : ACTkDetectorBase<ObscuredCheatingDetector>
	{
		public const string ComponentName = "Obscured Cheating Detector";
		internal const string LogPrefix = ACTk.LogPrefix + ComponentName + ": ";

		/// <summary>
		/// Holds detailed information about latest triggered detection.
		/// </summary>
		/// Can be null if there were no detections or if detection was triggered manually.
		public ObscuredCheatingDetectionInfo LastDetectionInfo { get; private set; }
		
		#region public fields
		
		/// <summary>
		/// Creates fake decrypted values for cheaters to find and hack it, triggering cheating attempts detection.
		/// </summary>
		/// Disable to make it harder to reveal obscured variables in memory, or keep enabled to catch more casual cheaters.
		[Tooltip("Creates fake decrypted values for cheaters to find and hack it, triggering cheating attempts detection.\n" +
				 "Disable to make it harder to reveal obscured variables in memory, or keep enabled to catch more casual cheaters.")]
		public bool honeyPot = true;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in \link ObscuredTypes.ObscuredDouble ObscuredDouble\endlink. Increase in case of false positives.
		/// </summary>
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredDouble. Increase in case of false positives.")]
		public double doubleEpsilon = 0.0001d;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in \link ObscuredTypes.ObscuredFloat ObscuredFloat\endlink. Increase in case of false positives.
		/// </summary>
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.")]
		public float floatEpsilon = 0.0001f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in \link ObscuredTypes.ObscuredVector2 ObscuredVector2\endlink. Increase in case of false positives.
		/// </summary>
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.")]
		public float vector2Epsilon = 0.1f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in \link ObscuredTypes.ObscuredVector3 ObscuredVector3\endlink. Increase in case of false positives.
		/// </summary>
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.")]
		public float vector3Epsilon = 0.1f;

		/// <summary>
		/// Max allowed difference between encrypted and fake values in \link ObscuredTypes.ObscuredQuaternion ObscuredQuaternion\endlink. Increase in case of false positives.
		/// </summary>
		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.")]
		public float quaternionEpsilon = 0.1f;
		#endregion

		#region public static methods
		/// <summary>
		/// Creates new instance of the detector at scene if it doesn't exists. Make sure to call NOT from Awake phase.
		/// </summary>
		/// <returns>New or existing instance of the detector.</returns>
		public static ObscuredCheatingDetector AddToSceneOrGetExisting()
		{
			return GetOrCreateInstance;
		}

		/// <summary>
		/// Starts all Obscured types cheating detection for detector you have in scene.
		/// </summary>
		/// Make sure you have properly configured detector in scene with #autoStart disabled before using this method.
		public static ObscuredCheatingDetector StartDetection()
		{
			if (Instance != null)
			{
				return Instance.StartDetectionInternal(null);
			}

			Debug.LogError(LogPrefix + "can't be started since it doesn't exists in scene or not yet initialized!");
			return null;
		}

		/// <summary>
		/// Starts all Obscured types cheating detection with specified callback.
		/// </summary>
		/// If you have detector in scene make sure it has empty Detection Event.<br/>
		/// Creates a new detector instance if it doesn't exists in scene.
		/// <param name="callback">Method to call after detection.</param>
		public static ObscuredCheatingDetector StartDetection(Action callback)
		{
			return GetOrCreateInstance.StartDetectionInternal(callback);
		}

		/// <summary>
		/// Stops detector. Detector's component remains in the scene. Use Dispose() to completely remove detector.
		/// </summary>
		public static void StopDetection()
		{
			if (Instance != null) Instance.StopDetectionInternal();
		}

		/// <summary>
		/// Stops and completely disposes detector component.
		/// </summary>
		/// On dispose Detector follows 2 rules:
		/// - if Game Object's name is "Anti-Cheat Toolkit Detectors": it will be automatically
		/// destroyed if no other Detectors left attached regardless of any other components or children;<br/>
		/// - if Game Object's name is NOT "Anti-Cheat Toolkit Detectors": it will be automatically destroyed only
		/// if it has neither other components nor children attached;
		public static void Dispose()
		{
			if (Instance != null) Instance.DisposeInternal();
		}
		#endregion
		
#if UNITY_EDITOR
		// making sure it will reset statics even if domain reload is disabled
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void SubsystemRegistration()
		{
			Instance = null;
		}
#endif
		
		internal static bool ExistsAndIsRunning => Instance && Instance.IsRunning;
		internal static bool IsRunningInHoneypotMode => ExistsAndIsRunning && Instance.honeyPot;

		private ObscuredCheatingDetector() {} // prevents direct instantiation
		
		/// <summary>
		/// Manually triggers cheating detection and invokes assigned events.
		/// </summary>
		[ContextMenu("Trigger detection")]
		public void TriggerDetection()
		{
			if (!IsRunning)
			{
				Debug.LogWarning(LogPrefix + "Detector is not running, can't trigger detection.");
				return;
			}

			LastDetectionInfo = new ObscuredCheatingDetectionInfo(typeof(ObscuredString), false,
				"This is an example of encrypted string.", "This is an example of modified string.");
			OnCheatingDetected();
		}
		
		[Obsolete]
		internal void OnCheatingDetected(IObscuredType type, object decrypted, object fake)
		{
			LastDetectionInfo = new ObscuredCheatingDetectionInfo(type.GetType(), true, decrypted, fake);
			OnCheatingDetected();
		}
		
		// boxing all around, eeek! =D
		// but it's not in the hot path and get called only once per session if cheating was detected, so who cares?
		internal void OnCheatingDetected(IObscuredType type, bool hashValid, object decrypted, object fake)
		{
			LastDetectionInfo = new ObscuredCheatingDetectionInfo(type.GetType(), hashValid, decrypted, fake);
			OnCheatingDetected();
		}
		
		private ObscuredCheatingDetector StartDetectionInternal(Action callback)
		{
			if (IsRunning)
			{
				Debug.LogWarning(LogPrefix + "already running!", this);
				return this;
			}

			if (!enabled)
			{
				Debug.LogWarning($"{LogPrefix}disabled but {nameof(StartDetection)} still called from somewhere (see stack trace for this message)!", this);
				return this;
			}

			LastDetectionInfo = null;
			
			if (callback != null && DetectorHasListeners())
			{
				Debug.LogWarning(LogPrefix + $"has properly configured Detection Event in the inspector or {nameof(CheatDetected)} event subscriber, but still get started with Action callback." +
								 $"Action will be called at the same time with Detection Event or {nameof(CheatDetected)} on detection." +
								 "Are you sure you wish to do this?", this);
			}

			if (callback == null && !DetectorHasListeners())
			{
				Debug.LogWarning($"{LogPrefix}was started without Detection Event, Callback or {nameof(CheatDetected)} event subscription." +
								 $"Cheat will not be detected until you subscribe to {nameof(CheatDetected)} event.", this);
			}

			if (callback != null)
				CheatDetected += callback;

			IsStarted = true;
			IsRunning = true;

			return this;
		}

		protected override void StartDetectionAutomatically()
		{
			StartDetectionInternal(null);
		}
		
		public static void TryDetectCheating<T, TU, TA, T2>(TU type, bool hashValid, TA hash, bool honeypotValid, T real, T2 fake) where TU : IObscuredType
		{
			if ((honeypotValid || !IsRunningInHoneypotMode) && hashValid)
				return;
			
#if ACTK_DETECTION_BACKLOGS || DEBUG
			Debug.LogError(LogPrefix + "Detection backlog:\n" +
							 $"type: {type.GetType()}\n" +
							 $"decrypted: {real}\n" +
							 $"fakeValue: {fake}\n" +
							 $"hashValid: {hashValid}\n" +
							 $"honeypotValid: {honeypotValid}\n" +
							 $"hash: {hash}");
#endif
			if (!Instance || Instance.IsCheatDetected)
				return;

			Instance.OnCheatingDetected(type, hashValid, real, fake);
		}
	}
}