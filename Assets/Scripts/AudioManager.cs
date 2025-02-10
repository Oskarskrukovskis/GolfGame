using UnityEngine;

public class AudioManager : MonoBehaviour
{
	private static AudioManager instance;
	public AudioSource generalAudioSource;
	public AudioSource ballAudioSource;
	public AudioSource holeAudioSource;
	public AudioSource coinAudioSource;

	public AudioClip ballMoveSound;
	public AudioClip ballPutSound;
	public AudioClip ballReadySound;
	public AudioClip redHoleSound;
	public AudioClip blueHoleSound;
	public AudioClip coinCollectSound;
	public AudioClip startGameSound;
	public AudioClip gameOverSound;


	public static AudioManager Instance
	{
		get
		{
			if (instance == null)
				instance = FindFirstObjectByType(typeof(AudioManager)) as AudioManager;

			return instance;
		}
		set
		{
			instance = value;
		}
	}
}
