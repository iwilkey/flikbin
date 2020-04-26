using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NewProject : MonoBehaviour
{
	public Image fade;
	private float t = 1.0f;
	public string name = "";
	public Font font;

	private string workBoard;

	private bool loading = false;

	void Update(){
		if(!loading){
			if(t > 0.0f){
				t -= 0.01f;
				fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, t);
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
								ES3.Save<string>("Work Board", name);
								t= 0.0f;
								loading = true;
							}
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

	bool checkLegality(string name){
		return true;
	}

    void OnGUI(){
    	name = GUI.TextField(new Rect((Screen.width / 4), Screen.height / 2 - 25, Screen.width / 2, 50), name, 40);
    	GUI.skin.textField.fontSize = 30;
    	GUI.skin.textField.alignment = TextAnchor.MiddleCenter;
    	GUI.skin.textField.font = font;

    }
}
