using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public Image pencil, thumbnail, next, prev;
    public Text projectName;
    private Vector3 sp;
    private RectTransform pencilT;
    public Sprite newProjectThumb;
    private float time, y = 90, t = 0, z = 0, t2 = 0, t3 = 0;
   	public bool switchThumbnail = false, loading = false;

   	private int counter = 0;
   	private string currentProject;
   	private List<string> existingProjects;
   	private string workBoard;

    void Start(){

    	loading = false;
    	pencilT = pencil.GetComponent<RectTransform>();
    	pencilT.localEulerAngles = new Vector3(0, y, 0);
    	sp = pencilT.position;
    	thumbnail.GetComponent<RectTransform>().sizeDelta = new Vector2(thumbnail.GetComponent<RectTransform>().sizeDelta.x, Screen.height * 0.52f);
    	thumbnail.fillAmount = 0;

    	next.color = new Color(next.color.r, next.color.g, next.color.b, t3);
    	prev.color = new Color(prev.color.r, prev.color.g, prev.color.b, t3);
    	projectName.color = new Color(projectName.color.r, projectName.color.g, projectName.color.b, t3);

    	thumbnail.sprite = newProjectThumb;

    	if(!ES3.KeyExists("Existing Projects")){
			existingProjects = new List<string>();
			ES3.Save<List<string>>("Existing Projects", existingProjects);
			existingProjects.Add("New Project");
		} else {
			existingProjects = ES3.Load<List<string>>("Existing Projects");
			existingProjects.Insert(0, "New Project");
			foreach(var proj in existingProjects){
				Debug.Log(proj);
			}
		}
    }

    void Update(){
    	if(!loading){

	    	counter = counter % existingProjects.Count;

	    	time = Time.time;

	    	float until, final, ss, es;
	    	until = 1.0f; final = 2.0f;
	    	ss = 0.3f; es = 0.05f;

	    	if(time > 0.0f && time <= until){
	    		y -= ss;
	    	} else if (time > until && time <= final){
	    		y -= es;
	    	} else {
	    		y -= ss;
	    	}

	    	pencilT.localEulerAngles = new Vector3(0, y, 0);

	    	float tm = 2.0f;
	    	Vector3 targ = new Vector3((Screen.width / 2), 145, 0);
	    	Vector3 targScale = new Vector3(0.4f, 0.4f, 0.4f);
	    	if(time > final){
	    		t += Time.deltaTime / tm;
	    		pencilT.position = Vector3.Lerp(sp, targ, Mathf.SmoothStep(0.0f, 1.0f, t));
	    		pencilT.localScale = Vector3.Lerp(new Vector3(1,1,1), targScale, Mathf.SmoothStep(0.0f, 1.0f, t));
	    	}

	    	if(time >= final + 2.0f && time <= final + 4.0f){
	    		switchThumbnail = true;

	    		if(t3 < 1.0f){
	    			t3 += 0.1f;	
	    		}
	    		next.color = new Color(next.color.r, next.color.g, next.color.b, t3);
	    		prev.color = new Color(prev.color.r, prev.color.g, prev.color.b, t3);
	    		projectName.color = new Color(projectName.color.r, projectName.color.g, projectName.color.b, t3);

	    	} else if(time > final + 4.0f) {
	    		if(counter - 1 < 0){
	    			prev.color = new Color(prev.color.r, prev.color.g, prev.color.b, 0.1f);
		    	} else {
		    		prev.color = new Color(prev.color.r, prev.color.g, prev.color.b, 1.0f);
		    	}

		    	if(counter + 1 > existingProjects.Count){
		    		next.color = new Color(next.color.r, next.color.g, next.color.b, 0.1f);
		    	} else {
		    		next.color = new Color(next.color.r, next.color.g, next.color.b, 1.0f);
		    	}
	    	}

	    	float tmS = 0.2f;
	    	if(switchThumbnail){
	    		t2 += Time.deltaTime / tmS;
	    		thumbnail.fillAmount = Mathf.SmoothStep(0.0f, 1.0f, t2);
	    	}

	    	currentProject = existingProjects[counter];
	    	projectName.text = currentProject;
	    }

    	if(loading){
    		t3 -= 0.01f;	
    		next.color = new Color(next.color.r, next.color.g, next.color.b, t3);
    		prev.color = new Color(prev.color.r, prev.color.g, prev.color.b, t3);
    		projectName.color = new Color(projectName.color.r, projectName.color.g, projectName.color.b, t3);
    		pencil.color = new Color(pencil.color.r, pencil.color.g, pencil.color.b, t3);
    		thumbnail.color = new Color(thumbnail.color.r, thumbnail.color.g, thumbnail.color.b, t3);

    		if(t3 <= 0.0f){
    			if(currentProject != "New Project"){
    				loading = false;
    				SceneManager.LoadScene("Editor");
    			} else {
    				loading = false;
    				SceneManager.LoadScene("New Project");
    			}
    		}
    	}

    	if(Input.GetMouseButtonDown(0)){
			Vector3 fingerPosition = Input.mousePosition;
			fingerPosition.z = Mathf.Infinity;
			RaycastHit2D hit = Physics2D.Raycast(fingerPosition, fingerPosition - Camera.main.ScreenToWorldPoint(fingerPosition), Mathf.Infinity);
			if(hit.collider != null){
				switch(hit.collider.gameObject.name){
					case "Next":

						if(!(counter + 1 > existingProjects.Count)){

							switchThumbnail = false;
							thumbnail.fillAmount = 0;


							counter++;
							counter = counter % existingProjects.Count;
							currentProject = existingProjects[counter];
							projectName.color = new Color(projectName.color.r, projectName.color.g, projectName.color.b, 1.0f);
							projectName.text = existingProjects[counter];
							if(currentProject != "New Project"){
								Texture2D pic = ES3.LoadImage("Project " + currentProject + " Frame 1 image.jpg");
								Sprite sprite = Sprite.Create(pic, new Rect(0.0f, 0.0f, pic.width, pic.height), new Vector2(0.5f, 0.5f));
								thumbnail.sprite = sprite;
							} else {
								thumbnail.sprite = newProjectThumb;
							}


							t2 = 0;
							switchThumbnail = true;
						}
						break;

					case "Previous":
						if(!(counter - 1 < 0)){
							switchThumbnail = false;
							thumbnail.fillAmount = 0;

							counter--;
							counter = counter % existingProjects.Count;
							currentProject = existingProjects[counter];
							projectName.color = new Color(projectName.color.r, projectName.color.g, projectName.color.b, 1.0f);
							projectName.text = existingProjects[counter];
							if(currentProject != "New Project"){
								Texture2D pic = ES3.LoadImage("Project " + currentProject + " Frame 1 image.jpg");
								Sprite sprite = Sprite.Create(pic, new Rect(0.0f, 0.0f, pic.width, pic.height), new Vector2(0.5f, 0.5f));
								thumbnail.sprite = sprite;
							} else {
								thumbnail.sprite = newProjectThumb;
							}
						}



						t2 = 0;
						switchThumbnail = true;
						break;

					case "Edit":

						if(time > 6.0f){
							workBoard = currentProject;
							ES3.Save<string>("Work Board", workBoard);
							loading = true;

						}

						break;
				}
			}
		}

    }
}
