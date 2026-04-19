using UnityEngine;

public class Station : MonoBehaviour
{
    public int stationType;
    public bool IsOccupied { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = GameController.Instance.IconsForType[stationType + 1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetStation()     {
        IsOccupied = false;
    }

    public void Occupy()     {
        if (IsOccupied) return;
        IsOccupied = true;
        GameController.Instance.GetComponent<AudioController>().PlayTrainArrived();
    }
}
