using UnityEngine;

public class RTSCamera : MonoBehaviour
{
    public float PanSpeedMultiplier = 1f;

    private void Update()
    {
        float panSpeedModifier = PanSpeedMultiplier;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) panSpeedModifier *= 2;

        transform.Translate(panSpeedModifier * transform.right * Input.GetAxis("Horizontal"), Space.World);
        transform.Translate(panSpeedModifier * transform.forward * Input.GetAxis("Vertical"), Space.World);
    }
}
