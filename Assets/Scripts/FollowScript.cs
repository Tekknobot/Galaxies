using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Transform target; // Reference to the target (e.g., spacecraft)

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target reference is missing!");
            return;
        }

        // Directly set the camera position to the target's position
        transform.position = target.position;

        // Make the camera look at the target
        transform.LookAt(target);
    }
}
