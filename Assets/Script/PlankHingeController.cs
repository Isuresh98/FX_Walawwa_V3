using UnityEngine;

public class PlankHingeController : MonoBehaviour
{
    public Rigidbody connectedBody;

    [Header("Hinge Settings")]
    public float anchorYOffset = 0.004943351f;
    public float limitMin = 7.24f;
    public float limitMax = 97.7f;

    private enum HingeSide { None, Left, Right }
    private HingeSide currentHingeSide = HingeSide.None;

    private HingeJoint hingeJoint;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleHinge(HingeSide.Left);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleHinge(HingeSide.Right);
        }
    }

    public void ActivateLeftHinge()
    {
        ToggleHinge(HingeSide.Left);
    }

    public void ActivateRightHinge()
    {
        ToggleHinge(HingeSide.Right);
    }

    void ToggleHinge(HingeSide side)
    {
        if (hingeJoint == null)
        {
            // No hinge currently – add one
            AddHinge(side);
            currentHingeSide = side;
        }
        else
        {
            // Hinge exists – remove it regardless of side
            RemoveHinge();
            currentHingeSide = HingeSide.None;
        }
    }

    void AddHinge(HingeSide side)
    {
        if (connectedBody != null)
            connectedBody.isKinematic = false;

        hingeJoint = gameObject.AddComponent<HingeJoint>();

        hingeJoint.axis = new Vector3(0, 0, 1);

        // Set anchor position
        Vector3 anchor = hingeJoint.anchor;
        anchor.y = (side == HingeSide.Left) ? -anchorYOffset : anchorYOffset;
        hingeJoint.anchor = anchor;

        // Set limits
        JointLimits limits = hingeJoint.limits;
        if (side == HingeSide.Left)
        {
            limits.min = Mathf.Abs(limitMin);
            limits.max = Mathf.Abs(limitMax);
        }
        else // Right
        {
            limits.min = -Mathf.Abs(limitMax);
            limits.max = -Mathf.Abs(limitMin);
        }
       
    }

    void RemoveHinge()
    {
        if (hingeJoint != null)
        {
            Destroy(hingeJoint);
            hingeJoint = null;
        }
    }
}