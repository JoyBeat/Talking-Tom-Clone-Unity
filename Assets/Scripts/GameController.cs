using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerState
{
	Idle,
	Listening,
	Talking
}

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour {

	private Animator _playerAnimator;
	private PlayerState _playerState = PlayerState.Idle;
	private AudioSource _audioSource;

	private float[] _clipSampleData;

	// Use this for initialization
	void Start () {
		var playerGameObject = GameObject.FindGameObjectWithTag(GameConstants.PlayerTag);
		if(playerGameObject != null)
		{		
			_playerAnimator = playerGameObject.GetComponent<Animator>();
		}

		_audioSource = GetComponent<AudioSource>();
		_clipSampleData = new float[GameConstants.SampleDataLength]/*1024*/;
		Idle();
	}
	
	// Update is called once per frame
	private void Update ()
    {
        /*
         * 1- if is in IdleState and the volume is above thresold 
         * we want to swith to the listen state
         */

		if(_playerState == PlayerState.Idle && IsVolumeAboveThresold())
		{
			SwitchState();
		}
	}

	private bool IsVolumeAboveThresold()
	{
        if(_audioSource.clip == null)
        {
            return false;
        }

		_audioSource.clip.GetData(_clipSampleData, _audioSource.timeSamples); //Read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
		var clipLoudness = 0f;
		foreach (var sample in _clipSampleData)
		{
			clipLoudness += Mathf.Abs(sample);
		}
		clipLoudness /= GameConstants.SampleDataLength; 

		Debug.Log("Clip Loudness = " + clipLoudness);

		return clipLoudness > GameConstants.SoundThreshold;/*0.025f*/
    }


	private void SwitchState()
	{
			switch(_playerState)
			{
				case PlayerState.Idle:
				_playerState = PlayerState.Listening;
				Listen();
				break;

				case PlayerState.Listening:
				_playerState = PlayerState.Talking;
				Talk();
				break;

				case PlayerState.Talking:
				_playerState = PlayerState.Idle;
				 Idle();
				break;
			}
	}


	private void Idle()
	{
        /*
         * 1- Play Idle animation
         * 2- Reset sound after playback 
         * 3- Contineously record the sound with the lowest duration possible 
         */

        if (_playerAnimator != null)
		{
			_playerAnimator.SetTrigger(GameConstants.MecanimIdle);

            if (_audioSource.clip != null)// if playback happened
            {
			    _audioSource.Stop();
			    _audioSource.clip = null;
            }
			_audioSource.clip = Microphone.Start(GameConstants.MicrophoneDeviceName/*null*/, true, GameConstants.IdleRecordingLength/*1 sec*/, GameConstants.RecordingFrequency/*44100*/);
		}	
	}


	private void Listen()
	{
        /*
         * 1- Play Listen animation
         * 2- Start recording user sound
         * 3- Transition to talking state after some time
         */

		if(_playerAnimator != null)
		{
			_playerAnimator.SetTrigger(GameConstants.MecanimListen);

			_audioSource.clip = Microphone.Start(GameConstants.MicrophoneDeviceName /*null*/, false, GameConstants.RecordingLength/*5*/, GameConstants.RecordingFrequency/*44100*/);
			Invoke("SwitchState", GameConstants.RecordingLength/*5 sec*/);
		}	
	}


	private void Talk()
	{
        /*
         * 1- Play Talk animation
         * 2- Stop Recording
         * 3- Play recorded sound
         * 3- Transition to Idle after the playback
         */

        if (_playerAnimator != null)
		{
			_playerAnimator.SetTrigger(GameConstants.MecanimTalk);

			Microphone.End(null);
			if(_audioSource.clip != null)
			{
				_audioSource.Play();
			}
		
			Invoke("SwitchState", GameConstants.RecordingLength);
		}	
	}

}
