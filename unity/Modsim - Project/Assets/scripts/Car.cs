using UnityEngine;
using DG.Tweening;

public class Car : MonoBehaviour
{

    private GameObject roundabout;
    private Roundabout roundaboutScript;

    private float radius = 0.0f;

    public float speed = 2.0f;
    private float curSpeed = 0.0f;
    public float curAngle = 0.0f;
    public float acceleration = 0.01f;

    public float minDistance = 2.0f;

    public bool isDebugCar = false;

    private static class Path
    {
        public static Vector3[] Roundabout = new Vector3[] {
            new Vector3( -5.877852522924734f , 0f,8.090169943749473f ),
            new Vector3( 5.877852522924732f, 0f , 8.090169943749475f ),
            new Vector3( 9.510565162951535f, 0f , 3.0901699437494745f ),
            new Vector3( 9.510565162951536f, 0f , -3.0901699437494736f ),
            new Vector3( 5.877852522924733f, 0f , -8.090169943749473f ),
            new Vector3( 1.2246467991473533e-15f, 0f , -10.0f ),
            new Vector3( -5.87785252292473f, 0f , -8.090169943749476f ),
            new Vector3( -9.510565162951535f, 0f , -3.0901699437494754f ),
            new Vector3( -9.510565162951536f, 0f , 3.0901699437494723f ),
            new Vector3( -5.877852522924734f, 0f , 8.090169943749473f )
        };
    }

    private Tween myTween;

    // Use this for initialization
    void Start()
    {
        roundabout = GameObject.FindWithTag("roundabout");
        roundaboutScript = roundabout.GetComponent<Roundabout>();
        updateRadius();

        this.transform.position = Path.Roundabout[0];
        myTween = this.transform.DOPath(Path.Roundabout, 5f, PathType.CatmullRom, PathMode.Full3D, 10, Color.green);
        myTween.SetSpeedBased();
        myTween.SetLoops(-1);
        myTween.SetEase(Ease.Linear);
        myTween.OnUpdate(onUpdate);
    }

    private GameObject[] getCars()
    {
        return GameObject.FindGameObjectsWithTag("car");
    }

    public float progress()
    {
        return myTween.ElapsedPercentage(false);
    }

    public bool isInitiated()
    {
        return myTween != null;
    }

    private float percentageToLength(float percent)
    {
        return percent * myTween.PathLength();
    }

    private float distance(Car ca)
    {
        var ca_p = ca.progress();
        var p = this.progress();
        if (p > ca_p)
        {
            ca_p += 1;
        }
        return ca_p - p;
    }

    private Car getClosest()
    {
        var cars = getCars();
        Car closest = null;
        var closestDiff = -1f;
        foreach(var c in cars)
        {
            if (c != this.gameObject)
            {
                var ca = c.GetComponent<Car>();
                if (ca.isInitiated())
                {
                    var ca_p = distance(ca);
                    if (closestDiff == -1 || ca_p < closestDiff)
                    {
                        closestDiff = ca_p;
                        closest = ca;
                    }
                }
            }
        }
        return closest;
    }

    private void onUpdate()
    {
        var closest = getClosest();
        if (closest != null)
        {
            if (isDebugCar)
            {
                Debug.Log(percentageToLength(distance(closest)));
                if (percentageToLength(distance(closest)) < 10f)
                {
                    myTween.Pause();
                }
            }
        }
    }

    private void updateRadius()
    {
        radius = roundaboutScript.radius;
    }
}