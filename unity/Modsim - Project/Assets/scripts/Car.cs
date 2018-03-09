using UnityEngine;
using DG.Tweening;

public class Car : MonoBehaviour
{
    private float radius = 0.0f;

    public float maxSpeed = 10f;
    public float acceleration = 0.5f;

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
        this.transform.position = Path.Roundabout[0];
        myTween = this.transform.DOPath(Path.Roundabout, 1f, PathType.CatmullRom, PathMode.Full3D, 10, Color.green);
        myTween.SetSpeedBased();
        myTween.SetLoops(-1);
        myTween.SetEase(Ease.Linear);
        myTween.OnUpdate(onUpdate);
        setSpeed(20);
    }

    private void setSpeed(float s)
    {
        myTween.timeScale = s;
    }

    private float getSpeed()
    {
        return myTween.timeScale;
    }

    private void addSpeed(float s)
    {
        s += getSpeed();
        if (s < 0)
        {
            s = 0;
        }
        if (s > maxSpeed)
        {
            //s = maxSpeed;
        }
        setSpeed(s);
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
        var diff = percent * myTween.PathLength();
        return diff;
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
                    if (closest == null || ca_p < closestDiff)
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
        var diff = minDistance;
        Debug.Log(myTween.PathLength());
        if (closest != null)
        {
            diff = percentageToLength(distance(closest));

            if (isDebugCar)
            {
                Debug.Log("PLEASE don't die one me");
            }
        }
        if (diff < minDistance)
        {
            setSpeed(0);
        }
        else
        {
            addSpeed(acceleration);
        }
    }

    public void FixedUpdate()
    {
    }
}