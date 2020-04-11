using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

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
		if(WebCamTexture.devices != null){
			if(!FlikittCore.getCurrentFrame().getHasPicture()){
				if(!webTex.isPlaying) webTex.Play();
			} else {
				if(webTex.isPlaying) webTex.Stop();
			}
		}
	}

	public void Capture(){
		webTex.Pause();
		Texture2D picTex = new Texture2D(camRender.texture.width, camRender.texture.height, TextureFormat.ARGB32, false);
		picTex.SetPixels(webTex.GetPixels());
		picTex.Apply();
		FlikittCore.getCurrentFrame().setHasPicture(true);
		FlikittCore.getCurrentFrame().setPicture(picTex);
	}

	public void DeleteCapture(){
		FlikittCore.getCurrentFrame().setPicture(null);
		FlikittCore.getCurrentFrame().setHasPicture(false);
	}
}
