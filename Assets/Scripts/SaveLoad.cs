using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
	FlikittCore FlikittCore;
	DrawingManager DrawingManager;
	UserInterface UserInterface;
	public GameObject linePrefab;

	void Start(){
		FlikittCore = GameObject.Find("Flikitt Core").GetComponent<FlikittCore>();
		DrawingManager = GameObject.Find("Drawing Manager").GetComponent<DrawingManager>();
		UserInterface = GameObject.Find("User Interface").GetComponent<UserInterface>();
	}

	public void Save(string name) {

		string find = "Project " + name;

		//Save name
		if(ES3.KeyExists((name + " name"))) ES3.DeleteKey((name + " name"));	
		string nm = FlikittCore.project.getName();
		ES3.Save<string>(name + " name", nm);

		//Save size
		if(ES3.KeyExists((name + " size"))) ES3.DeleteKey((name + " size"));
		int size = FlikittCore.project.getAllFrames().Count;
		ES3.Save<int>(name + " size", size);

		//Save all images
		foreach(var frame in FlikittCore.project.getAllFrames()){
			if(ES3.KeyExists(("Project " + name + " " + frame.getName() + " image.jpg"))) { ES3.DeleteKey(("Project " + name + " " + frame.getName() + " image.jpg")); }
			Texture2D pic = frame.getPicture() as Texture2D;

			ES3.SaveImage(pic, "Project " + name + " Frame " + frame.getNumber() + " image.jpg");
		}

		//Save all lines
		List<Frame> frames = FlikittCore.project.getAllFrames();
		foreach(Frame frm in frames){
			Transform frameT = frm.getGOSelf().transform;
			string frmname = frameT.gameObject.name;
			int childCount = frameT.childCount;
			List<string> lineNames = new List<string>();
			ES3.Save<int>("Project " + name + " " + frameT.gameObject.name + " Child count", childCount);
			for(int i = 0; i < childCount; i++){
				GameObject ln = frameT.GetChild(i).gameObject;
				LineRenderer lr = ln.GetComponent<LineRenderer>();
				List<Vector3> points = new List<Vector3>();

				for(int p = 0; p < lr.positionCount; p++){
					points.Add(lr.GetPosition(p));
				}

				lineNames.Add(frameT.GetChild(i).gameObject.name);

				float thickness = lr.startWidth;
				Color lineColor = lr.startColor;

				ES3.Save<float>("Project " + name + " " + frmname + " " + frameT.GetChild(i).gameObject.name + " Thickness", thickness);
				ES3.Save<Color>("Project " + name + " " + frmname + " " + frameT.GetChild(i).gameObject.name + " Color", lineColor);
				ES3.Save<List<Vector3>>("Project " + name + " " + frmname + " " + frameT.GetChild(i).gameObject.name + " Points", points);
			}
			ES3.Save<List<string>>("Project " + name + " " + frmname + " Line names", lineNames);
		}

		//Save Audio, if there is audio, of course.
		if(FlikittCore.project.getAllAudio() != null){
			if(FlikittCore.project.getAllAudio().Count >= 1 && FlikittCore.project.checkAudioCompletion()){

				int trackCount = FlikittCore.project.getAllAudio().Count;

				int counter = 1;
				foreach(var clip in FlikittCore.project.getAllAudio()){
					if(ES3.KeyExists("Project " + name + " Track " + counter.ToString())) ES3.DeleteKey("Project " + name + " Track " + counter.ToString());
					ES3.Save<AudioClip>("Project " + name + " Track " + counter.ToString(), clip);
					counter++;
				}

				ES3.Save<int>("Project " + name + " Track Amount", trackCount);
			}
		}

		//Save fps
		if(ES3.KeyExists((name + " fps"))) ES3.DeleteKey((name + " fps"));
		float fps = FlikittCore.project.getFps();
		ES3.Save<float>(name + " fps", fps);

	}

	public Project Load(string name){
		if(ES3.KeyExists(name + " name")){

			//Load size
			int size = ES3.Load<int>(name + " size");

			//Load all the images
			List<Texture2D> images = new List<Texture2D>();
			for(int i = 1; i <= size; i++){
				Texture2D pic = ES3.LoadImage("Project " + name + " Frame " + i + " image.jpg");
				images.Add(pic);
			}

			//Make frames out of lines
			List<Frame> frames = new List<Frame>();
			for(int i = 1; i <= size; i++){
				Frame frame = new Frame(i);
				int childCount = ES3.Load<int>("Project " + name + " Frame " + i + " Child count");
				List<string> lineNms = ES3.Load<List<string>>("Project " + name + " Frame " + i + " Line names");
				for(int line = 1; line <= childCount; line++){
					DrawingManager.currentLine++;
					GameObject lin = Instantiate(linePrefab);
					lin.name = "Line " + DrawingManager.currentLine;

					LineRenderer lr = lin.GetComponent<LineRenderer>();

					Color lineColor = ES3.Load<Color>("Project " + name + " Frame " + i + " " + lineNms[line - 1] + " Color");
					float lineThickness = ES3.Load<float>("Project " + name + " Frame " + i + " " + lineNms[line - 1] + " Thickness");
					List<Vector3> linePoints = ES3.Load<List<Vector3>>("Project " + name + " Frame " + i + " " + lineNms[line - 1] + " Points");

					lr.SetColors(lineColor, lineColor);
					lr.SetWidth(lineThickness, lineThickness);
					lr.positionCount = linePoints.Count;

					for(int d = 0; d < lr.positionCount; d++){
						lr.SetPosition(d, linePoints[d]);
					}

					lin.transform.parent = frame.getGOSelf().transform;
				}

				frame.setPicture(images[i - 1] as Texture);
				frame.setHasPicture(true);

				frames.Add(frame);
			}

			//Load Audio
			List<AudioClip> audio = new List<AudioClip>();
			int trackCount = ES3.Load<int>("Project " + name + " Track Amount");
			if(ES3.KeyExists("Project " + name + " Track 1")){
				for(int i = 1; i <= trackCount; i++){
					string n = ("Project " + name + " Track " + i);
					AudioClip clip = ES3.Load<AudioClip>(n);
					audio.Add(clip);
				}
			} else {
				audio = null;
			}

			//Load fps
			float fps = ES3.Load<float>(name + " fps");

			Project project = new Project(name, "Frame-by-frame", frames, audio, fps);
			FlikittCore.project = project;
			UserInterface.fpsSlider.value = fps;

			return project;
			
		} else {
			Debug.Log("Did not find project name!");
			return null;
		}
	}
}