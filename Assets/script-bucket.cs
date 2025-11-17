using UnityEngine;

public class BucketAngleTrigger : MonoBehaviour
{
    public float angleThreshold = 60f; 
    public ParticleSystem particles;

    bool isPlaying = false;

    void Update()
    {
        float angle = Vector3.Angle(transform.up, Vector3.up);
        Debug.Log("Angle du seau : " + angle);

        if (angle > angleThreshold && !isPlaying)
        {
            particles.Play();
            isPlaying = true;
        }
        else if (angle <= angleThreshold && isPlaying)
        {
            particles.Stop();
            isPlaying = false;
        }
    }
}