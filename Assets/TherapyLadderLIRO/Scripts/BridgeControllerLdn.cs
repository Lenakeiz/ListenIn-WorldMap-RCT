using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BridgeControllerLdn : MonoBehaviour {

    HingeJoint2D jointController;
    public float motorSpeed;
    Rigidbody2D rb2d;
    public float rotationAngle = 20.0f;
    public float rotationSpeed = 1.0f;
    public float timeForTransition = 2.0f;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        jointController = GetComponent<HingeJoint2D>();
        jointController.useMotor = false;
        
    }

    void ForceDisableMotor()
    {
        jointController.useMotor = false;
        //rb2d.bodyType = RigidbodyType2D.Static;
    }
    //True is opening, false is closing
    public void SetBridgeOpenState(bool state)
    {
        //rb2d.bodyType = RigidbodyType2D.Dynamic;
        //JointMotor2D motor = jointController.motor;
        //motor.motorSpeed = state ? motorSpeed : -1 * motorSpeed;
        //jointController.useMotor = true;
        //jointController.motor = motor;        
        //Invoke("ForceDisableMotor", 1.5f);

        RotateTween(state);

    }

    private void RotateTween(bool rotation)
    {
        float finalAngle = rotation ? rotationAngle : 0.0f;
        float initialAngle = rotation ? 0.0f : rotationAngle;
        iTween.ValueTo(this.gameObject, iTween.Hash("from", initialAngle, "to", finalAngle, "time", timeForTransition, "easetype", iTween.EaseType.easeInCubic, "onupdate", "UpdatePosition", "onupdatetarget", this.gameObject));
    }

    public void UpdatePosition(float v)
    {
        transform.rotation = Quaternion.Euler(0, 0, v);
    }

    private IEnumerator Rotate(bool rotation)
    {
        float finalAngle = rotation ? rotationAngle : 0.0f;
        float initialAngle = rotation ? 0.0f : finalAngle;

        float t = 0.0f;
        float currAngle = 0.0f;
        while (t < 1.0f)
        {
            currAngle = Mathf.Lerp(initialAngle, finalAngle, t);
            t += Time.deltaTime * rotationSpeed;
            yield return new WaitForEndOfFrame();
        }

    }

}
