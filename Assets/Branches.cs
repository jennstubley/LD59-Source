using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Branches : MonoBehaviour
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
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == signal.Value);
        }

    }

    public void OnMouseDown()
    {
        //  Don't allow signal changes while trains are running.
        if (GameController.Instance.IsPlaying) return;
        GameController.Instance.GetComponent<AudioController>().PlaySwichClicked();
        signal.Value = (signal.Value + 1) % transform.childCount;
    }
}
