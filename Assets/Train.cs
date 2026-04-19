using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class Train : MonoBehaviour
{
    [SerializeField] private TrainPath startPath;
    [SerializeField] private float startPosition = 0; // T value on the current spline.
    [SerializeField] private int trainType;
    [SerializeField] private float junctionCooldown = 0f;

    [SerializeField] private TrainPath currentPath;
    [SerializeField] private float currentPosition; // t on the current spline.

    [SerializeField] private bool running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPath = startPath;
        currentPosition = startPosition;
        transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = GameController.Instance.IconsForType[trainType + 1];
        UpdateTrainVisual();
    }

    // Update is called once per frame
    void Update()
    {
        if (!running)
        {
            return;
        }
        if (!currentPath.ShouldMove(currentPosition))
        {
            running = false;
            return;
        }
        UpdateTrainVisual();
        currentPosition = currentPath.ClampPosition(currentPath.NormalizedMove(currentPosition, Time.deltaTime, 4f));
        if (junctionCooldown > 0f)
        {
            junctionCooldown -= Time.deltaTime;
        }
    }

    public bool IsRunning()
    {
        return running;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Train train = other.gameObject.GetComponent<Train>();
        if (train != null && IsRunning())
        {
            Debug.Log("Train collision!");
            running = false;
            train.running = false;
            GameController.Instance.GetComponent<AudioController>().PlayCrash();
            return;
        }

        Junction junction = other.gameObject.GetComponent<Junction>();
        if (junction != null)
        {
            HandleJunction(junction);
            return;
        }

        Station station = other.gameObject.GetComponent<Station>();
        if (station != null)
        {
            HandleStation(station);
            return;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Train train = collision.gameObject.GetComponent<Train>();
        if (train != null && IsRunning())
        {
            Debug.Log("Train collision!");
            running = false;
            train.running = false;
            GameController.Instance.GetComponent<AudioController>().PlayCrash();
        }
    }

    private void HandleStation(Station station)
    {
        if (trainType == station.stationType)
        {
            Debug.Log("Train arrived at station!");
            station.Occupy();
            running = false;
        }
    }

    private void HandleJunction(Junction junction)
    {
        if (junctionCooldown > 0f)
        {
            return; // Still in cooldown, ignore junction triggers.
        }
        if (junction != null)
        {
            Debug.Log("Train going through junction!");
            TrainPath oldPath = currentPath;
            currentPath = junction.GetNextPath(currentPath, trainType);
            UpdatePosition(oldPath);
            junctionCooldown = 0.1f; // Prevent immediately re-triggering the junction.
        }

        return;
    }

    private void UpdatePosition(TrainPath oldPath)
    {
        if (currentPath.SplineIndex == oldPath.SplineIndex && currentPath.Direction == oldPath.Direction)
        {
            // Same path, no need to update position.
            return;
        }
        Vector3 localSplinePoint = currentPath.SplineContainer.transform.InverseTransformPoint(transform.position);
        SplineUtility.GetNearestPoint(currentPath.SplineContainer.Splines[currentPath.SplineIndex], localSplinePoint, out float3 nearest, out currentPosition, SplineUtility.PickResolutionDefault, 2);
    }

    private void UpdateTrainVisual()
    {
        float3 pos, tangent;
        currentPath.Evaluate(currentPosition, out pos, out tangent);
        transform.position = pos;
        transform.up = tangent * currentPath.Direction;
    }

    public void StartRunning()
    {
        running = true;
    }

    public void ResetTrain()
    {
        currentPath = startPath;
        currentPosition = startPosition;
        running = false;
        UpdateTrainVisual();
    }
}

[Serializable]
public class TrainPath
{
    public SplineContainer SplineContainer;
    public int SplineIndex = 0;
    public int Direction = 1; // 1 for forward, -1 for backward

    public bool ShouldMove(float t)
    {
        return (t <= 1f && t >= 0) || SplineContainer.Splines[SplineIndex].Closed;
    }

    public void Evaluate(float t, out float3 position, out float3 tangent)
    {
        float3 upVector;
        SplineContainer.Evaluate(SplineIndex, t, out position, out tangent, out upVector);
    }

    public float ClampPosition(float t)
    {
        if (SplineContainer.Splines[SplineIndex].Closed)
        {
            while (t > 1f)
            {
                t -= 1f;
            }
            while (t < 0f)
            {
                t += 1f;
            }
        }
        return t;
    }

    internal static TrainPath Reverse(TrainPath entrance)
    {
        return new TrainPath
        {
            SplineContainer = entrance.SplineContainer,
            SplineIndex = entrance.SplineIndex,
            Direction = -entrance.Direction
        };
    }

    internal float NormalizedMove(float currentPosition, float deltaTime, float speed)
    {
        return currentPosition + (Direction * deltaTime * (1 / (SplineContainer.Splines[SplineIndex].GetLength() / speed)));
    }
}
