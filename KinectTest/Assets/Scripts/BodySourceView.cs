using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;
using Joint = Windows.Kinect.Joint;

public class BodySourceView : MonoBehaviour 
{
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    
    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType>
    {
        JointType.Head,
        //JointType.HandLeft,
        //JointType.HandRight,
    };

    void Update () 
    {
        #region Get Kinect Data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
        {
            return;
        } 

        List<ulong> trackedIDs = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }
            if (body.IsTracked)
            {
                trackedIDs.Add(body.TrackingId);
            }
        }
        #endregion
        
        #region Delete Kinect Bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIDs.Contains(trackingId))
            {
                Destroy(mBodies[trackingId]);
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect Bodies
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                if (!mBodies.ContainsKey(body.TrackingId))
                {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion
    }
    
    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);

        foreach (JointType joint in _joints)
        {
            GameObject newJoint = Instantiate(mJointObject);
            Debug.Log("Instantiated Object");
            newJoint.name = joint.ToString();

            newJoint.transform.parent = body.transform; 
        }

        return body;
    }
    
    private void UpdateBodyObject(Body body, GameObject bodyObject)
    {
        foreach (JointType _joint in _joints)
        {
            Joint sourceJoint = body.Joints[_joint];
            Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
            targetPosition.z *= -1;

            Transform jointObject = bodyObject.transform.Find(_joint.ToString());
            jointObject.position = targetPosition;
        }
    }
    private static Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
