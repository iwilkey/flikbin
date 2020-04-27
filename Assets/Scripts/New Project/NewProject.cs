using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class NewProject : MonoBehaviour
{
	public Image fade;
	private float t = 1.0f;
	public string name = "";
	public Font font;
	public Text error;

	private string workBoard;
	private List<string> existingProjects;

	private bool loading = false;

	void Start(){
		if(ES3.KeyExists("Existing Projects")){
			existingProjects = ES3.Load<List<string>>("Existing Projects");
		} else {
			existingProjects = new List<string>();
		}

		error.text = "";
	}

	void Update(){
		if(!loading){
			if(t > 0.0f){
				t -= 0.01f;
				fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, t);
			}

			if(!checkLegality(name) && name != ""){
				error.text = "You cannot use this name!";
			} else if (checkLegality(name)  && name != "") {
				error.text = "";
			}

			if(Input.GetMouseButtonDown(0)){
				Vector3 fingerPosition = Input.mousePosition;
				fingerPosition.z = Mathf.Infinity;
				RaycastHit2D hit = Physics2D.Raycast(fingerPosition, fingerPosition - Camera.main.ScreenToWorldPoint(fingerPosition), Mathf.Infinity);
				if(hit.collider != null){
					switch(hit.collider.gameObject.name){
						case "Create":
							if(name != "" && checkLegality(name)){
								workBoard = name;
								ES3.Save<string>("Work Board", name.ToLower());
								t= 0.0f;
								loading = true;
							} else if (name == "") { 
								error.text = "Please input a name for your project!";
							}
							break;

						case "Back":
							SceneManager.LoadScene("Splash Screen");
							break;
					}
				}
			}
		} else {
			if(t < 1.0f){
				t += 0.01f;
				fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, t);
			} else {
				SceneManager.LoadScene("Editor");
			}
		}
	}

	bool checkLegality(string _name){
		_name = name.ToLower();
		//Check if the name already exists...
		if(existingProjects.Contains(_name)) return false;

		return true;
	}

    void OnGUI(){
    	name = GUI.TextField(new Rect((Screen.width / 4), Screen.height / 2 - 25, Screen.width / 2, 50), name, 40);
    	name = Regex.Replace(name, @"[^a-zA-Z0-9 ]", "");
    	GUI.skin.textField.fontSize = 30;
    	GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
    	GUI.skin.textField.font = font;
    }
}
