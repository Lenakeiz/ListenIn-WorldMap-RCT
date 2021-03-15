using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class BridgeController : MonoBehaviour {

    HingeJoint2D jointController;
    public float motorSpeed;
    Rigidbody2D rb2d;

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

    public void SetBridgeOpenState(bool state)
    {
        //rb2d.bodyType = RigidbodyType2D.Dynamic;
        JointMotor2D motor = jointController.motor;
        motor.motorSpeed = state ? motorSpeed : -1 * motorSpeed;
        jointController.useMotor = true;
        jointController.motor = motor;
        Invoke("ForceDisableMotor", 1.5f);
    }

}
