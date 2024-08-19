using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

namespace Nothke.Audio
{
    public class CollisionSounds : MonoBehaviour
    {
        const float RELATIVE_VELOCITY_TRESHOLD = 0.1f;

        // impulse based, currently unused: 
        //const float IMPULSE_VOLUME_MULT = 0.00001f;
        //const float IMPULSE_THRESHOLD = 1;

        public CollisionSoundsProfile profile;

        public float volumeVelocityMult = 0.05f;
        public float timeout = 0.05f;
        public float pitchMult = 1;
        float lastTime;

        public bool preventDoubleSound;

        public AudioMixerGroup mixerGroup;

#if UNITY_EDITOR
        private void Start()
        {
            if (!profile)
                Debug.LogError("No collision sounds profile assigned", this);
        }
#endif

        public void ResetTimeout()
        {
            lastTime = Time.time;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!enabled)
                return;

            // Prevent sound in the first second
            float time = Time.time;
            if (time < 1)
                return;

            if (timeout > 0)
            {
                if (time - lastTime < timeout)
                    return;

                lastTime = time;
            }

            if (preventDoubleSound)
            {
                var colSound = collision.collider.GetComponent<CollisionSounds>();

                if (colSound)
                    colSound.ResetTimeout();
            }

            //float impulse = (collision.impulse / Time.fixedDeltaTime).magnitude;
            //Debug.Log("Impulse: " + impulse);

            //if (impulse < IMPULSE_THRESHOLD) return;

            var relVel = collision.relativeVelocity.magnitude;

            if (relVel < RELATIVE_VELOCITY_TRESHOLD)
                return;

            Debug.Assert(profile, "No collision sounds profile assigned", this);

            // TODO: Move parameters to AudioManager:
            profile.clips.Play(collision.GetContact(0).point,
                volume: relVel * volumeVelocityMult,
                pitch: Random.Range(0.95f, 1.05f) * pitchMult,
                minDistance: 10, mixerGroup: mixerGroup);
        }
    }
}