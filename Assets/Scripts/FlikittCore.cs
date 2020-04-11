using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame{

	private string name;
	private bool enabled, hasPicture;
	private GameObject goSelf;
	private RawImage imageSelf;

	public Frame(int _number){
		name = "Frame " + _number;

		goSelf = new GameObject(name, typeof(RectTransform));
		goSelf.gameObject.tag = "Frame";
		goSelf.transform.parent = GameObject.Find("Project").transform;
		
		RectTransform rT = goSelf.GetComponent<RectTransform>();
		rT.SetAnchor(AnchorPresets.StretchAll);
		rT.SetTop((int)((Screen.height-Screen.width) / 2));
		rT.SetBottom((int)((Screen.height-Screen.width) / 2));
		rT.SetLeft((int)((Screen.width-Screen.height) / 2));
		rT.SetRight((int)((Screen.width-Screen.height) / 2));
		rT.localEulerAngles = new Vector3(0,0,-90);

		goSelf.AddComponent<RawImage>();
		imageSelf = goSelf.GetComponent<RawImage>();

		hasPicture = false;
		enabled = true;
	}

	public void Enable(){
		if(!enabled){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.CompareTag("Frame") && obj.name == this.name){
					obj.gameObject.SetActive(true);
					enabled = true;
				}
			}
		} else {
			return;
		}
	}

	public void Disable(){
		if(enabled){
			foreach(var obj in Resources.FindObjectsOfTypeAll<Transform>() as Transform[]){
				if(obj.CompareTag("Frame") && obj.name == this.name){
					obj.gameObject.SetActive(false);
					enabled = false;
				}
			}
		} else {
			return;
		}
	}

	public void setName(int num) {name = "Frame " + num;}
	public string getName() {return name;}
	public bool getHasPicture(){return hasPicture;}
	public void setHasPicture(bool p) {hasPicture = p;}
	public void setPicture(Texture image) {imageSelf.texture = image;}
	public bool isEnabled() {return enabled;}
	public void setStatus(string status){
		if(status == "On"){
			Enable();
		} else if (status == "Off"){
			Disable();
		} else {
			return;
		}
	}
	public GameObject getGOSelf() {return goSelf;}
}

public class FlikittCore : MonoBehaviour
{
	public int currentFrame = 1;
	public List<Frame> frames;
	public bool isPlaying;
	public float spf;

	void Start(){
		//Is for a new project...
		if(frames == null){
			frames = new List<Frame>();
			NewPage();
		}

		isPlaying = false;
		spf = 1.0f;
	}

	public void NewPage(){
		DisableAll();
		currentFrame++;
		Frame frame = new Frame(currentFrame);
		frames.Add(frame);
		EnableActive(currentFrame);
	}

	public void LoadPage(int frame){
		DisableAll();
		currentFrame = frame;
		EnableActive(currentFrame);
	}

	void DisableAll(){
		for(int i = 0; i < frames.Count; i++){
			frames[i].setStatus("Off");
		}
	}

	void EnableActive(int currentFrame){
		for(int i = 0; i < frames.Count; i++){
			if(frames[i].getName() == "Frame " + currentFrame){
				frames[i].setStatus("On");
			}
		}
	}

	public Frame getCurrentFrame(){
		for(int i = 0; i <= frames.Count; i++){
			if(frames[i].getName() == (string)("Frame " + currentFrame.ToString())){
				return frames[i];
			}
		}
		return null;
	}

	private IEnumerator coroutine;
	public void StartPlay(){
		if(frames.Count > 1){
			LoadPage(1);
			coroutine = Play();
			StartCoroutine(coroutine);
		} else {
			return;
		}
	}

	private IEnumerator Play(){
		for (int i = 1; i <= frames.Count; i++){
			DisableAll();
			EnableActive(i);
			currentFrame = i;

			if(isPlaying){
				yield return new WaitForSeconds(spf);
			} else {
				break;
			}
		}
		if(isPlaying){
			StartPlay();
		} else {
			LoadPage(1);
			yield break;
		}
	}
}