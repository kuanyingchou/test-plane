using UnityEngine;
using System.Collections;

public class KZ2DRotator : MonoBehaviour {
	private Vector3 lastPosition;
	public Vector3 pivot;
	public void OnTouchBegan(KZTouchEvent e) {
		lastPosition = e.touch.position;
	}
	public void OnTouchMoved(KZTouchEvent e) {
		Vector3 currentPosition = e.touch.position;
		Vector3 screenPivot = Camera.main.WorldToScreenPoint(pivot);
		float currentAngle = GetAngle (currentPosition - screenPivot);
		float lastAngle = GetAngle (lastPosition - screenPivot);
		transform.RotateAround(pivot,Vector3.forward,currentAngle-lastAngle);
		lastPosition = currentPosition;
	}
	private float GetAngle(Vector3 input){
		return Mathf.Atan2(input.y,input.x) * Mathf.Rad2Deg;
	}
	
}
