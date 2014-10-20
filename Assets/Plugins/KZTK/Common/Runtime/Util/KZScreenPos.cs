using UnityEngine;
using System.Collections;
/// <summary>
/// 螢幕物件對齊螢幕上下左右
/// </summary>
public class KZScreenPos : MonoBehaviour {
	
	public enum TYPE	{
		TOP,
		CENTER,
		BOTTOM,
		LIFTTOP,
		LIFTCENTER,
		LIFTBOTTOM,
		RIGHTTOP,
		RIGHTCENTER,
		RIGHTBOTTOM
	}

	Vector3 _pos = new Vector3 (0,0,0);
	public float offset_x = 0;
	public float offset_Y = 0;
	public bool _SetLiveUpdate = false;
	public Camera tmpCam;
	public TYPE _type ;
	public int x_offset = 1;
	public int y_offset = 1;

	/// <summary>
	/// 是否可以update更新
	/// </summary>
	/// <value><c>true</c> if set live update; otherwise, <c>false</c>.</value>
	public bool SetLiveUpdate	{
		get { return _SetLiveUpdate; }
		set { _SetLiveUpdate = value; }
	}
	void Awake()
	{
		SetVector3 ();
		SetScreenPosition(1,1);
	}
	void Start()	{
		
	}
	void Update () 	{
		if(_SetLiveUpdate){
			SetVector3 ();
			SetScreenPosition(1,1);
		}	
	}
	/// <summary>
	/// 依照所選的type 改變  上下左右的位置
	/// </summary>
	void SetVector3()	{
		switch(_type)
		{
		case KZScreenPos.TYPE.BOTTOM://0,1
			_pos = new Vector3 (Screen.width/2,0,0);
			x_offset = 0;
			y_offset = 1;
			break;
		case KZScreenPos.TYPE.CENTER://0,0
			_pos = new Vector3 (Screen.width/2,Screen.height/2,0);
			x_offset = 0;
			y_offset = 0;
			break;
		case KZScreenPos.TYPE.LIFTBOTTOM://1,1
			_pos = new Vector3 (0,0,0);
			x_offset = 1;
			y_offset = 1;
			break;
		case KZScreenPos.TYPE.LIFTCENTER://1,0
			_pos = new Vector3 (0,Screen.height/2,0);
			x_offset = 1;
			y_offset = 0;
			break;
		case KZScreenPos.TYPE.LIFTTOP://1, -1
			_pos = new Vector3 (0,Screen.height,0);
			x_offset = 1;
			y_offset = -1;
			break;
		case KZScreenPos.TYPE.RIGHTBOTTOM://-1,1
			_pos = new Vector3 (Screen.width,0,0);
			x_offset = -1;
			y_offset = 1;
			break;
		case KZScreenPos.TYPE.RIGHTCENTER://-1,0
			_pos = new Vector3 (Screen.width,Screen.height/2,0);
			x_offset = -1;
			y_offset = 0;
			break;
		case KZScreenPos.TYPE.RIGHTTOP://-1,-1
			_pos = new Vector3 (Screen.width,Screen.height,0);
			x_offset = -1;
			y_offset = -1;
			break;
		case KZScreenPos.TYPE.TOP://0,-1
			_pos = new Vector3 (Screen.width/2,Screen.height,0);
			x_offset = 0;
			y_offset = -1;
			break;
		default:
			Debug.Log (".......................");
			break;
		}
	}
	/// <summary>
	/// 真正改變物件的螢幕位置
	/// </summary>
	void SetScreenPosition(float mX,float mY)
	{
		if(!tmpCam)tmpCam = gameObject.transform.root.GetComponent<Camera>();
		if(!tmpCam)tmpCam = Camera.main;

		Vector3 temp = tmpCam.ScreenToWorldPoint(_pos);
		float x = temp.x+(offset_x*mX);
		float y = temp.y+(offset_Y*mY);
		float z = gameObject.transform.position.z ;

		gameObject.transform.position = new Vector3( x , y, z);
	}
	/// <summary>
	/// 動態改變TYPE
	/// </summary>
	/// <param name="type">Type.</param>
	public void SetScreenPosType(Camera _cam,KZScreenPos.TYPE type)
	{
		_type = type;
		SetCamera(_cam);
		SetVector3 ();
		SetScreenPosition(1,1);
	}
	public void SetScreenPosType(Camera _cam,KZScreenPos.TYPE type,float _offset_x,float _offset_y)
	{
		_type = type;
		SetCamera(_cam);
		SetOffsetPos(_offset_x,_offset_y);
		SetVector3 ();
		SetScreenPosition(x_offset,y_offset);
	}
	void SetCamera(Camera _cam)
	{
		tmpCam = _cam;
	}
	void SetOffsetPos(float _x, float _y)
	{
		offset_x = _x;
		offset_Y = _y;
	}


}
