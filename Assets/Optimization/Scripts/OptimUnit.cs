using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class OptimUnit : MonoBehaviour {
    private Vector3 currentVelocity;
    private float timeToVelocityChange;
    private float currentAngularVelocity;
    private float timeToAngularVelocityChange;

    private Vector3 areaSize;

    public void SetAreaSize(Vector3 size) {
        areaSize = size;
    }

    private void Start() {
        PickNewVelocity();
        PickNewVelocityChangeTime();
        PickNewAngularVelocity();
        PickNewAngularVelocityChangeTime();
    }

    private Vector3 rotateValue = new Vector3();

    // Update is called once per frame
    private void Update() {
        Profiler.BeginSample("Handling Time");
        HandleTime();
        Profiler.EndSample();

        Profiler.BeginSample("Rotating");

        var currentPosition = transform.position;

        if (currentPosition.x <= 0)
            rotateValue.x = currentAngularVelocity * Time.deltaTime;
        else if (currentPosition.x > 0)
            rotateValue.x = -currentAngularVelocity * Time.deltaTime;

        if (currentPosition.z >= 0)
            rotateValue.z = currentAngularVelocity * Time.deltaTime;
        else if (currentPosition.z < 0)
            rotateValue.z = -currentAngularVelocity * Time.deltaTime;

        transform.Rotate(rotateValue);

        Profiler.EndSample();

        Profiler.BeginSample("Moving");
        Move();
        Profiler.EndSample();

        Profiler.BeginSample("Boundary Check");

        //check if we are moving away from the zone and invert velocity if this is the case
        if (currentPosition.x > areaSize.x && currentVelocity.x > 0) {
            currentVelocity.x *= -1;
            PickNewVelocityChangeTime(); //we pick a new change time as we changed velocity
        } else if (currentPosition.x < -areaSize.x && currentVelocity.x < 0) {
            currentVelocity.x *= -1;
            PickNewVelocityChangeTime();
        }

        if (currentPosition.z > areaSize.z && currentVelocity.z > 0) {
            currentVelocity.z *= -1;
            PickNewVelocityChangeTime(); //we pick a new change time as we changed velocity
        } else if (currentPosition.z < -areaSize.z && currentVelocity.z < 0) {
            currentVelocity.z *= -1;
            PickNewVelocityChangeTime();
        }
        Profiler.EndSample();
    }


    private void PickNewVelocity() {
        currentVelocity = Random.insideUnitSphere;
        currentVelocity.y = 0;
        currentVelocity *= 10.0f;
    }

    private void PickNewAngularVelocity() {
        currentAngularVelocity = Random.Range(-180.0f, 180.0f);
    }

    private void PickNewVelocityChangeTime() {
        timeToVelocityChange = Random.Range(2.0f, 5.0f);
    }

    private void PickNewAngularVelocityChangeTime() {
        timeToAngularVelocityChange = Random.Range(2.0f, 5.0f);
    }

    private void Move() {
        //Vector3 position = transform.position;

        //float distanceToCenter = Vector3.Distance(Vector3.zero, position);
        //float speed = 0.5f + distanceToCenter / areaSize.magnitude;

        //int steps = Random.Range(1000, 2000);
        //float increment = Time.deltaTime / steps;
        //for (int i = 0; i < steps; ++i) {
        //    position += currentVelocity * increment * speed;
        //}

        transform.position = transform.position + currentVelocity * Time.deltaTime;
    }

    private void HandleTime() {
        timeToVelocityChange -= Time.deltaTime;
        if (timeToVelocityChange < 0) {
            PickNewVelocity();
            PickNewVelocityChangeTime();
        }

        timeToAngularVelocityChange -= Time.deltaTime;
        if (timeToAngularVelocityChange < 0) {
            PickNewAngularVelocity();
            PickNewAngularVelocityChangeTime();
        }
    }
}
