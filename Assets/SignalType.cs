using UnityEngine;

public class SignalType : MonoBehaviour
{
    private Signal signal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        signal = GetComponentInParent<Signal>();

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sprite = GameController.Instance.IconsForType[signal.Type + 1];
    }

    public void OnMouseDown()
    {
        //  Don't allow signal changes while trains are running.
        if (GameController.Instance.IsPlaying) return;

        GameController.Instance.GetComponent<AudioController>().PlaySwichClicked();
        // Branch script handles changes to value.
        signal.Type = (signal.Type + 1);
        if (signal.Type >= GameController.Instance.IconsForType.Count - 1)
        {
            signal.Type = -1;
        }
    }
}
