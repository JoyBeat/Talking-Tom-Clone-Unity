using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants  {
	public const string PlayerTag = "Player";
	public const string GameControllerTag = "GameController";
	public const string MecanimTalk = "Talk";
	public const string MecanimListen= "Listen";
	public const string MecanimIdle = "Idle";
	public const string MicrophoneDeviceName = null;// "Built-in Microphone";

	public const int IdleRecordingLength = 1;
	public const int RecordingLength = 5;

	public const int RecordingFrequency = 44100;

	public const int SampleDataLength = 1024;

	public const float SoundThreshold = 0.025f;
}
