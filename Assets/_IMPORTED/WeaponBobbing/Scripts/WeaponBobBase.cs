using UnityEngine;

namespace WeaponBobbing {
	public abstract class WeaponBobBase : MonoBehaviour {
		public float bobMultiplier = 1.0f;	// Use this when you have another script to lerping weapon position.
		protected const float INTERNAL_MULTIPLIER = 1.0f;
		protected Vector3 initPos;
		protected Vector3 initRot;
		protected float timer = 0;
		public Transform rig;
		public LocalRotator rotator;

        protected virtual void Start() {
			initPos = transform.localPosition;

			if(rig != null) {
				initRot = rig.localRotation.eulerAngles;
			}

			if(rotator != null) {
				rotator.enabled = true;
			}
		}

		void OnEnable() {
			if(rotator != null) {
				rotator.enabled = true;
			}
		}

		void OnDisable() {
			if(rotator != null) {
				rotator.enabled = false;
			}
		}
	}	
}