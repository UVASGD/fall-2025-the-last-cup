using UnityEngine;


public class GooglyEyes : MonoBehaviour {
	public float rPupil = 0.5f;
	public float rEye = 1f;

	public float speed = 1f;
	public float gravMultiplier = 1f;
	public float Bounciness = 0.4f;
	public float Friction = 0.4f;

	private Vector3 LastForward;
	private Vector3 LastPosition;
	private Vector3 AppliedVelocity = Vector3.zero;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start() {
		this.LastPosition = this.transform.parent.position;
	}

	// Update is called once per frame
	void Update() {
		float maxDist = rEye - rPupil;

		var CurrentPosition = this.transform.parent.position;
		var Gravity =  this.transform.InverseTransformDirection(Physics.gravity);

		this.AppliedVelocity += Gravity * this.gravMultiplier * Time.deltaTime;
		this.AppliedVelocity += this.transform.InverseTransformDirection(this.LastPosition - CurrentPosition) * 500f * Time.deltaTime;
		this.AppliedVelocity.z = 0;

		var EyePos = this.transform.localPosition;
		EyePos += this.AppliedVelocity * this.speed * Time.deltaTime;
		if (EyePos.magnitude > maxDist) {
			var Normal = -EyePos.normalized;
			this.AppliedVelocity = Vector3.Reflect(this.AppliedVelocity, Normal);
			var normalComponent = Vector3.Project(this.AppliedVelocity, Normal);
			var tangetComponent = this.AppliedVelocity - normalComponent;
			this.AppliedVelocity = normalComponent * this.Bounciness + tangetComponent * Friction;
			EyePos = EyePos.normalized * maxDist;
		}
		EyePos.z = this.transform.localPosition.z;
		this.transform.localPosition = EyePos;
		this.LastPosition = this.transform.parent.position;


		


	}
}
