using System;
using UnityEngine;

public class Junction : MonoBehaviour
{

    [SerializeField] private TrainPath Entrance;
    [SerializeField] private TrainPath[] Branches;
    [SerializeField] private Signal Signal;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public TrainPath GetNextPath(TrainPath trainPath, int trainType)
    {
        if (trainPath.SplineIndex == Entrance.SplineIndex && trainPath.Direction == Entrance.Direction)
        {
            // Using the entrance. Pick a branch based on the signal.
            if (Signal.Type == -1 || Signal.Type == trainType)
            {
                return Branches[Signal.Value];
            }
            else
            {
                // Use the opposite branch.
                return Branches[(Signal.Value + 1) % Branches.Length];
            }
        }
        else
        {
            // Coming from a branch so take the reverse of the entrance.
            return TrainPath.Reverse(Entrance);
        }
    }
}
