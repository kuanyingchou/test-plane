using UnityEngine;
using System.Collections;

public class KZLocale  
{

	private static string DEFAULT_LOCALE = "zh_TW";
	private static string appLocale = DEFAULT_LOCALE;
	public static string  GetSystemLocale()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR

		AndroidJavaClass languageAndroid  = new AndroidJavaClass("com.allproducts.kizi.plugins.util.Util");
		string locale = languageAndroid.CallStatic<string>("getLocale");		 
		return locale;	
		#elif UNITY_EDITOR
	
		return System.Globalization.CultureInfo.InstalledUICulture.Name.Replace("-","_") ;
                #else
                return DEFAULT_LOCALE;
		#endif

	}
	public static string  GetAppLocale()
	{
		return appLocale;
	}
	public static void SetAppLocale( string locale)
	{
		appLocale = locale;
	}

	public static Object Load(string resName)
	{
		Object obj =  Resources.Load(appLocale+"/"+resName);
		if(!obj)
		{
			Debug.LogError (string.Format( "在\"{0}\"找不到\"{1}\" , 改用\"{2}\" ",appLocale,resName,DEFAULT_LOCALE));
			obj =  Resources.Load(DEFAULT_LOCALE+"/"+resName);

		}
	
		return obj;
	}



}
