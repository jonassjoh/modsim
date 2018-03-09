using UnityEngine;
using System.Collections.Generic;

public class SplineWalker : MonoBehaviour {

	public BezierSpline spline;

	public float speed;

	public bool lookForward;

	public SplineWalkerMode mode;

	private float progress;
	private bool goingForward = true;

    public bool isTestCar = false;
    public SplineWalker otherCar;

	private void Update () {
        if (isTestCar)
        {
            Debug.Log(GetDistance(GetClosest()));
        }

        FollowPath();
	}

    public float GetDistance(SplineWalker car)
    {
        var steps = 50;
        var from = this.progress;
        var to = car.progress;
        if (to >= from)
        {
            return spline.GetPathLength(steps, from, to);
        }
        return spline.GetPathLength((int) (steps/2), from, 1f) + spline.GetPathLength((int) (steps/2), 0f, to);
    }

    public SplineWalker[] GetCars()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("car");
        List<SplineWalker> res = new List<SplineWalker>();
        foreach (var o in objs)
        {
            if (o != this)
            {
                var e = o.GetComponent<SplineWalker>();
                res.Add(e);
            }
        }
        return res.ToArray();
    }

    public SplineWalker GetClosest()
    {
        var cars = GetCars();
        SplineWalker closest = null;
        float closestDiff = 0;
        foreach (var car in cars)
        {
            var diff = GetDistance(car);
            if (closest == null || diff < closestDiff)
            {
                closest = car;
                closestDiff = diff;
            }
        }
        return closest;
    }

    private float GetSpeed()
    {
        return speed / 20000;
    }

    private void SetSpeed(float s)
    {
        speed = s;
    }

    private void FollowPath()
    {
        if (goingForward)
        {
            //progress += Time.deltaTime / duration;
            progress += spline.GetConstantTimeStep(progress) * GetSpeed();
            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.deltaTime / speed;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }
}