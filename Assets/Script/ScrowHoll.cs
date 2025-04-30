using System.Collections;
using UnityEngine;

public class ScrowHoll : MonoBehaviour
{
    public PlankHingeController plankHingeController;
    public bool isLeft;

    public Rigidbody nutrb;

    void Start()
    {

     
    }

    public void ActiveScruwHoll()
    {

        // Start the coroutine to delay the nut activation
        StartCoroutine(DelayedNutsActive());

        if (isLeft)
        {
            plankHingeController.ActivateLeftHinge();
        }
        else
        {
            plankHingeController.ActivateRightHinge();
        }

       
    }

    private IEnumerator DelayedNutsActive()
    {
        Collider collider = GetComponent<Collider>();
        collider.isTrigger = true;
        yield return new WaitForSeconds(0.1f);
        nustActive();
    }

    public void nustActive()
    {
#if UNITY_EDITOR
        Debug.Log("Nuts Active");
#endif
        nutrb.isKinematic = false;
        Collider rbcollider = nutrb.gameObject.GetComponent<Collider>();
        rbcollider.isTrigger = false;
        Destroy(this.gameObject, 2f);
       
    }
}