using System;
using System.Collections;
using UnityEngine;

namespace Nothke.Interaction.Items
{
    /// <summary>
    /// Use this if mass of rigidbody changes while the rigidbody is being held
    /// </summary>
    public interface IDragRigidbodyReleaseMassSettable
    {
        float ReleaseMass { get; }
    }

    public class DragRigidbody : MonoBehaviour
    {
        public static DragRigidbody e;
        private void Awake() { e = this; }

        public float k_Spring = 50.0f;
        public float k_Damper = 5.0f;
        public float k_Drag = 0f;
        public float k_AngularDrag = 0.0f;
        public float k_Distance = 0.0001f;
        public bool k_AttachToCenterOfMass = false;

        public float distanceLimit = 2;
        public float clampVelocity = 2;
        public float clampExitVelocity = 2;

        public float fakeMass = 0;
        float originalMass;

        Rigidbody body;
        private ConfigurableJoint joint;
        RigidbodyInterpolation originalInterpolation;

        public bool Holding => joint && joint.connectedBody;

        public event Action OnSlipped;

        public bool Hitting { private set; get; }

        float hitDistance;

        private void Start()
        {
            CreateJoint();
        }

        void CreateJoint()
        {
            var go = new GameObject("Rigidbody dragger");
            body = go.AddComponent<Rigidbody>();
            //body.rotation = Quaternion.LookRotation(transform.forward, transform.up);
            joint = go.AddComponent<ConfigurableJoint>();
            body.isKinematic = true;
        }

        public float GetJointDistance()
        {
            if (joint && joint.connectedBody)
                return Vector3.Distance(
                        body.position,
                        joint.connectedBody.transform.TransformPoint(joint.connectedAnchor));
            else
                return 0;
        }

        Quaternion addedRotation = Quaternion.identity;
        [NonSerialized] public bool rotate;
        [NonSerialized] public float rotateXInput;
        [NonSerialized] public float rotateYInput;

        bool overrideTarget = false;
        Vector3 overrideTargetPoint;

        public void OverrideTarget(Vector3 target)
        {
            overrideTarget = true;
            overrideTargetPoint = target;
        }

        public void EndOverridingTarget()
        {
            overrideTarget = false;
        }

        public Vector3 GetFrontTargetPoint()
        {
            return new Ray(transform.position, transform.forward).GetPoint(hitDistance);
        }

        void FixedUpdate()
        {
            if (joint && joint.connectedBody)
            {
                //var ray = new Ray(transform.position, transform.forward);

                Vector3 targetPoint = overrideTarget ? overrideTargetPoint :
                    GetFrontTargetPoint();
                //ray.GetPoint(hitDistance);

                joint.transform.position = targetPoint;

                //joint.connectedBody.velocity = Vector3.ClampMagnitude(joint.connectedBody.velocity, clampVelocity);
                //joint.targetRotation = transform.rotation;
                //joint.targetRotation = UnityEngine.Random.rotation;

                var bodyRot = body.rotation;
                if (rotate)
                {
                    float mx = rotateXInput * 10;
                    float my = rotateYInput * 10;

                    addedRotation =
                        Quaternion.AngleAxis(mx, Vector3.up) *
                        Quaternion.AngleAxis(my, Vector3.right) * addedRotation;


                    //addedRotation *= Quaternion.Euler(mx * 10, 0, my * 10);

                    //body.rotation = rot * addedRotation;
                    //body.MoveRotation(rot * Quaternion.Euler(4, 0, 0));
                    //joint.targetRotation = rot * Quaternion.Euler(4, 0, 0);
                }

                //Quaternion faceTo = Quaternion.Inverse(transform.rotation) * Quaternion.Euler(0, 90, 0);
                //addedRotation = Quaternion.Slerp(addedRotation, faceTo, Time.deltaTime * 10);

                body.rotation = transform.rotation * addedRotation;

                if (GetJointDistance() > distanceLimit)
                {
                    Slip();
                }

                return;
            }
        }

        public void Slip()
        {
            End();

            if (OnSlipped != null)
                OnSlipped();
        }

        /*
        private void Update()
        {
            if (Input.GetMouseButtonUp(0) && joint && joint.connectedBody)
                End();
        }*/

        public void Attach(RaycastHit hit, bool fixedRotation = true)
        {
            Attach(hit.rigidbody, hit.point, hit.distance, fixedRotation);
        }

        public void Attach(Rigidbody rigidbody, Vector3 point, float distance, bool fixedRotation)
        {
            if (!joint) CreateJoint();

            if (fakeMass > 0)
            {
                originalMass = rigidbody.mass;
                rigidbody.mass = fakeMass;
            }

            originalInterpolation = rigidbody.interpolation;
            rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

            joint.transform.position = point;
            joint.anchor = Vector3.zero;
            joint.transform.rotation = transform.rotation;
            //joint.targetRotation = Quaternion.identity;
            //joint.targetRotation = Quaternion.Inverse(transform.rotation);// Quaternion.LookRotation(transform.forward, transform.up);

            joint.angularXMotion = joint.angularYMotion = joint.angularZMotion
                = fixedRotation ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Free;

            JointDrive drive = new JointDrive();
            drive.positionSpring = k_Spring;
            drive.positionDamper = k_Damper;
            drive.maximumForce = Mathf.Infinity;

            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;

            //joint.spring = k_Spring;
            //joint.damper = k_Damper;
            //joint.maxDistance = k_Distance;
            joint.connectedBody = rigidbody;

            hitDistance = distance;
        }

        public void End()
        {
            if (!joint.connectedBody)
                return;

            if (fakeMass > 0)
            {
                var massSettable = joint.connectedBody.GetComponent<IDragRigidbodyReleaseMassSettable>();

                if (massSettable != null)
                {
                    joint.connectedBody.mass = massSettable.ReleaseMass;
                }
                else
                {
                    joint.connectedBody.mass = originalMass;
                }
            }

            joint.connectedBody.interpolation = originalInterpolation;

            if (clampExitVelocity > 0)
                joint.connectedBody.velocity =
                    Vector3.ClampMagnitude(joint.connectedBody.velocity, clampExitVelocity);

            addedRotation = Quaternion.identity;

            joint.connectedBody = null;

            overrideTarget = false;
        }
    }
}