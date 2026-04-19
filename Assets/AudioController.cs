using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip TrainArrivedClip;
    public AudioClip SwitchClip;
    public AudioClip RoundStartClip;
    public AudioClip CrashClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTrainArrived()
    {
        audioSource.PlayOneShot(TrainArrivedClip);
    }

    public void PlaySwichClicked()
    {
        audioSource.PlayOneShot(SwitchClip);
    }

    public void PlayRoundStart()
    {
        audioSource.PlayOneShot(RoundStartClip);
    }

    public void PlayCrash()
    {
        audioSource.PlayOneShot(CrashClip);
    }

}
