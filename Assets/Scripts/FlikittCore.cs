using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Project : MonoBehaviour {
	private string name, type;
	private List<Frame> frames;
	private AudioClip audio;
	private float fps;

	//Constructing a new project
	public Project(string _name, string _type){
		name = _name; 
		type = _type;
		frames = new List<Frame>();
		audio = null;
		fps = 7.5f;
	}

	//Loading an existing project
	public Project(string _name, string _type, List<Frame> _frames, AudioClip _audio, float _fps){
		name = _name;
		type = _type;
		frames = _frames;
		audio = _audio;
		fps = _fps;
	}

	//Accessor
	public string getName() {return name;}
	public string getType() {return type;}
	public AudioClip getAudio() {return audio;}
	public float getFps() {return fps;}
	public Frame getFrame(int number) {return frames[number];}
	public List<Frame> getAllFrames() {return frames;}

	//Mutator
	public void addFrame(Frame frame) {frames.Add(frame);}
	public void setAudio(AudioClip _audio) {audio = _audio;}
	public void setFps(float _fps) {fps = _fps;}

	public void deleteFrame(Frame frame){
		for (int i = 0; i < frames.Count; i++){
			if (frames[i].getName() == frame.getName()){
				Destroy(frames[i].getGOSelf());
				frames.RemoveAt(i);
			}
		}
	}

}

public class Frame {

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
		rT.localScale = new Vector3(1,1,1);

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
	public Project project;
	CameraManager CameraManager;

	public int currentFrame = 1;
	public bool isPlaying, continuousShot, isShooting;
	public float spf;
	public string drawMode;

	void Start(){
		CameraManager = GameObject.Find("Camera Manager").GetComponent<CameraManager>();
		//Use easysave to find out if a project is being loaded or not...

		//If a new project
		string name = "New project"; 
		string type = "Frame-by-Frame";
		project = new Project(name, type);
		NewPage();
		spf = 1 / project.getFps();

		drawMode = "Pencil";
	}

	void Update(){
		spf = 1 / project.getFps();
	}

	public void NewPage(){
		DisableAll();
		currentFrame++;
		Frame frame = new Frame(currentFrame);
		project.addFrame(frame);
		EnableActive(currentFrame);
	}

	public void LoadPage(int frame){
		DisableAll();
		currentFrame = frame;
		EnableActive(currentFrame);
	}

	void DisableAll(){
		for(int i = 0; i < project.getAllFrames().Count; i++){
			project.getFrame(i).setStatus("Off");
		}
	}

	void EnableActive(int currentFrame){
		for(int i = 0; i < project.getAllFrames().Count; i++){
			if(project.getFrame(i).getName() == "Frame " + currentFrame){
				project.getFrame(i).setStatus("On");
			}
		}
	}

	public Frame getCurrentFrame(){

		for(int i = 0; i <= project.getAllFrames().Count; i++){
			if(project.getFrame(i).getName() == (string)("Frame " + currentFrame.ToString())){
				return project.getFrame(i);
			}
		}
		return null;
	}

	private IEnumerator coroutine;
	public void StartPlay(){
		if(project.getAllFrames().Count > 1){
			LoadPage(1);
			coroutine = Play();
			StartCoroutine(coroutine);
		} else {
			return;
		}
	}

	private IEnumerator Play(){
		for (int i = 1; i <= project.getAllFrames().Count; i++){
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

	private IEnumerator contShotCoroutine;
	public void StartShot(){
		contShotCoroutine = Shoot();
		StartCoroutine(contShotCoroutine);
	}

	private IEnumerator Shoot(){
		if(isShooting){
			CameraManager.Capture();

			if(project.getAllFrames().Count >= currentFrame + 1){
				if(project.getFrame(currentFrame).getHasPicture()){
					CameraManager.DeleteSpecificCapture(currentFrame);
					LoadPage(currentFrame + 1);
				} else {
					LoadPage(currentFrame + 1);
				}
			} else {
				NewPage();
			}

			yield return new WaitForSeconds(spf);

			StartShot();
		} else {
			yield break;
		}
	}
}