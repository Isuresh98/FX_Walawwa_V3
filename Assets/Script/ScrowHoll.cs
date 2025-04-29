using UnityEngine;

public class ScrowHoll : MonoBehaviour
{
    public PlankHingeController plankHingeController;
    public bool isLeft;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ActiveScruwHoll()
    {

        Destroy(this.gameObject, 0.1f);

        if (isLeft)
        {
            plankHingeController.ActivateLeftHinge();
        }
        else
        {
            plankHingeController.ActivateRightHinge();
        }

       

        
    }
}