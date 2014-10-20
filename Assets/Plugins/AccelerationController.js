#pragma strict
 var zeroAc: Vector3;
 var curAc: Vector3;
 var sensH: float = 10;
 var sensV: float = 10;
 var smooth: float = 0.5;
static var GetAxisH: float = 0;
static var GetAxisV: float = 0;
 
 function ResetAxes(){
     zeroAc = Input.acceleration;
     curAc = Vector3.zero;
 }
 
 function Start(){
     ResetAxes();
 }
 
 function Update(){
     curAc = Vector3.Lerp(curAc, Input.acceleration-zeroAc, Time.deltaTime/smooth);
     GetAxisV = Mathf.Clamp(curAc.y * sensV, -1, 1);
     GetAxisH = Mathf.Clamp(curAc.x * sensH, -1, 1);
     // now use GetAxisV and GetAxisH instead of Input.GetAxis vertical and horizontal
     // If the horizontal and vertical directions are swapped, swap curAc.y and curAc.x
     // in the above equations. If some axis is going in the wrong direction, invert the
     // signal (use -curAc.x or -curAc.y)
 }
