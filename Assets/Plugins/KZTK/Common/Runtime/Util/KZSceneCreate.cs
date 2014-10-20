using UnityEngine;
using System.Collections;

public class KZSceneCreate  {

	/// <summary>
	/// 在場景上創造一個camera 只比暫停UI少一層  depth79  暫停UI depth80
	/// </summary>
	/// <returns>The scene camera.</returns>
	/// <param name="name">Name.</param>
	/// <param name="orthographicSize">Orthographic size.</param>
	/// <param name="depth">Depth.</param>
	/// <param name="vec">Vec.</param>
	/// <param name="picW">Pic w.</param>
	/// <param name="picH">Pic h.</param>
	/// <param name="autoCameraSizeRatio">If set to <c>true</c> auto camera size ratio.</param>
	public static GameObject CreateSceneCamera(string name,float orthographicSize  ,float depth,Vector3 vec,float picW,float picH,bool autoCameraSizeRatio)
	{
		GameObject cam = new GameObject();
		cam.transform.position = vec;
		cam.name = name;
		
		Camera camScr =  cam.AddComponent<Camera>();
		camScr.orthographic = true;
		camScr.orthographicSize = orthographicSize;
		camScr.depth = depth;
		camScr.clearFlags = CameraClearFlags.Depth;
		
		KZCameraAdjuster adjuster = cam.AddComponent<KZCameraAdjuster>();
		adjuster.bounds.width = picW;
		adjuster.bounds.height = picH;
		adjuster.update = autoCameraSizeRatio;
		return cam;
	}
	/// <summary>
	/// 創造一個單純的BG  z軸定在900;
	/// </summary>
	/// <returns>The scene B.</returns>
	/// <param name="sizeW">Size w.</param>
	/// <param name="sizeH">Size h.</param>
	/// <param name="mat">Mat.</param>
	/// <param name="cam">Cam.</param>
	/// <param name="type">Type.</param>
	public static GameObject CreateSceneBG(float sizeW,float sizeH,Material mat,
	                                       Camera cam,KZScreenPos.TYPE type = KZScreenPos.TYPE.CENTER)
	{
		GameObject bg = GameObject.CreatePrimitive (PrimitiveType.Plane);
		bg.renderer.material = mat;
		bg.transform.localPosition = new Vector3(0,0,900);
		bg.transform.eulerAngles=new Vector3 (90f,180f, 0f);
		bg.transform.localScale = new Vector3(sizeW,1,sizeH);
		bg.AddComponent<KZScreenPos>();
		bg.GetComponent<KZScreenPos>().SetScreenPosType(cam,type,sizeW/2,sizeH/2);
		bg.collider.enabled = false;
		return bg;
	}
	/// <summary>
	/// 創造一個borad  
	/// </summary>
	/// <returns>The scene broad.</returns>
	/// <param name="sizeW">Size w.</param>
	/// <param name="sizeH">Size h.</param>
	/// <param name="mat">Mat.</param>
	/// <param name="pos">Position.</param>
	/// <param name="cam">Cam.</param>
	/// <param name="type">Type.</param>
	public static GameObject CreateSceneBroad(float sizeW,float sizeH,Material mat,Vector3 pos,
	                                          Camera cam,KZScreenPos.TYPE type = KZScreenPos.TYPE.CENTER)
	{
		GameObject broad  = GameObject.CreatePrimitive (PrimitiveType.Plane);
		broad.transform.localScale =new Vector3(sizeW,1,sizeH);
		broad.transform.eulerAngles=new Vector3 (90f,180f, 0f);
		broad.AddComponent<KZScreenPos>();
		broad.GetComponent<KZScreenPos>().SetScreenPosType(cam,type,sizeW/2,sizeH/2);
		broad.transform.localPosition =pos;
		broad.renderer.material = mat;
		broad.collider.enabled = false;
		return  broad; 
	}
	/// <summary>
	/// 創造一個按鈕在場景上
	/// </summary>
	/// <returns>The scene button.</returns>
	/// <param name="name">Name.</param>
	/// <param name="parent">Parent.</param>
	/// <param name="mat">Mat.</param>
	/// <param name="W">W.</param>
	/// <param name="H">H.</param>
	/// <param name="cam">Cam.</param>
	/// <param name="type">Type.</param>
	public static GameObject CreateSceneBtn(string name,GameObject parent,
	                                        Material mat,float W,float H,Camera cam ,
	                                        KZScreenPos.TYPE type = KZScreenPos.TYPE.CENTER)
	{
		float _ScaleX = W*10;
		float _ScaleY = H*10;
		GameObject btn = new GameObject();
		btn.tag = "MBtn";
		btn.transform.parent = parent.transform;
		btn.name =name;
		btn.AddComponent<BoxCollider>();
		btn.GetComponent<BoxCollider>().center =new  Vector3(0,0,0); 
		btn.GetComponent<BoxCollider>().size =new Vector3(_ScaleX,_ScaleY,1);
		btn.AddComponent<KZScreenPos>();
		btn.GetComponent<KZScreenPos>().SetScreenPosType(cam,type,_ScaleX/2,_ScaleX/2);
		
		GameObject btnAnchor = new GameObject();
		btnAnchor.name = "btnAnchor";
		btnAnchor.transform.parent = btn.transform;
		btnAnchor.transform.localPosition =new Vector3((btn.GetComponent<KZScreenPos>().x_offset*_ScaleX*-1)/2,(btn.GetComponent<KZScreenPos>().y_offset*_ScaleY*-1)/2,-1);
		GameObject btnChild  = GameObject.CreatePrimitive (PrimitiveType.Plane);
		btnChild.name = "btnChild";
		btnChild.transform.parent = btnAnchor.transform;
		btnChild.transform.localScale =new Vector3(W,1,H);
		btnChild.transform.eulerAngles=new Vector3 (90f,180f, 0f);
		btnChild.transform.localPosition =new Vector3((btn.GetComponent<KZScreenPos>().x_offset*_ScaleX)/2,(btn.GetComponent<KZScreenPos>().y_offset*_ScaleY)/2,0);
		btnChild.renderer.material = mat;
		btnChild.collider.enabled = false;
		return btn;
	}
}

