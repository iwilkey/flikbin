using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public static class RectTransformExtensions
{
	public static void SetLeft(this RectTransform rt, float left)
	{
	 rt.offsetMin = new Vector2(left, rt.offsetMin.y);
	}

	public static void SetRight(this RectTransform rt, float right)
	{
	 rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
	}

	public static void SetTop(this RectTransform rt, float top)
	{
	 rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
	}

	public static void SetBottom(this RectTransform rt, float bottom)
	{
	 rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
	}
}

public class CameraManager : MonoBehaviour
{
	public RawImage camRender;
	WebCamTexture webTex = null;

	FlikittCore FlikittCore;

	void Start(){
		FlikittCore = GameObject.Find("Flikitt Core").GetComponent<FlikittCore>();

		//ANDROID STARTS HERE
		#if PLATFORM_ANDROID
		if(!Permission.HasUserAuthorizedPermission(Permission.Camera)){
			do {
				Permission.RequestUserPermission(Permission.Camera);
			} while (!Permission.HasUserAuthorizedPermission(Permission.Camera));
		}

		if(WebCamTexture.devices != null){
			string camName = WebCamTexture.devices[0].name;
			webTex = new WebCamTexture(camName, Screen.width, Screen.height);

			camRender.texture = webTex;
			camRender.material.mainTexture = webTex;
			camRender.rectTransform.SetTop((int)((Screen.height-Screen.width) / 2));
			camRender.rectTransform.SetBottom((int)((Screen.height-Screen.width) / 2));
			camRender.rectTransform.SetLeft((int)((Screen.width-Screen.height) / 2));
			camRender.rectTransform.SetRight((int)((Screen.width-Screen.height) / 2));
			camRender.rectTransform.localEulerAngles = new Vector3(0,0,-90);

			if(webTex != null) webTex.Play();
		}
		#endif
		//ANDROID ENDS HERE

	}

	void Update(){
		if(!FlikittCore.getCurrentFrame().getHasPicture()){
			if(!webTex.isPlaying) webTex.Play();
		} else {
			if(webTex.isPlaying) webTex.Stop();
		}
	}
}
