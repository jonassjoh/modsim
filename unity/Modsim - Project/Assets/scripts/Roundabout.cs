using UnityEngine;

public class Roundabout : MonoBehaviour
{

    public float radius = 10;
    public GameObject car;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Instantiate(car, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}