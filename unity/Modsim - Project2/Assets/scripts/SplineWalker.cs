using UnityEngine;

public class SplineWalker : MonoBehaviour {

	public BezierSpline spline;

	public float speed;

	public bool lookForward;

	public SplineWalkerMode mode;

	private float progress;
	private bool goingForward = true;

	private void Update () {
        FollowPath();
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