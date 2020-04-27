using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.ReplayKit;

public class ShareManager : MonoBehaviour
{
	UserInterface ui;
	FlikittCore fc;
	public bool isRecording;

	void Start(){
		ui = GameObject.Find("User Interface").GetComponent<UserInterface>();
		fc = GameObject.Find("Flikitt Core").GetComponent<FlikittCore>();
		init();
	}

	public bool init(){
		bool isRecordingAPIAvailable = ReplayKitManager.IsRecordingAPIAvailable();
		string message = isRecordingAPIAvailable ? "API Available on this mobile device!" : "API is NOT available on this mobile device.";
		Debug.Log(message);

		if(isRecordingAPIAvailable){
			ReplayKitManager.Initialise();
		}

		return isRecordingAPIAvailable;
	}

	void OnEnable()
	{
    	ReplayKitManager.DidInitialise += DidInitialise;
	}
	void OnDisable()
	{
	    ReplayKitManager.DidInitialise -= DidInitialise;
	}

	private  void  DidInitialise(ReplayKitInitialisationState  state,  string  message)  
	{  
	    Debug.Log("Received Event Callback : DidInitialise [State:"  +  state.ToString()  +  " "  +  "Message:"  +  message);  

	    switch  (state)  
	    {  
	        case  ReplayKitInitialisationState.Success:  
	            Debug.Log("ReplayKitManager.DidInitialise : Initialisation Success");  
	            break;  
	        case  ReplayKitInitialisationState.Failed:  
	            Debug.Log("ReplayKitManager.DidInitialise : Initialisation Failed with message["+message+"]");  
	            break;  
	        default:  
	            Debug.Log("Unknown State");  
	            break;  
	    }  
	}

	public void StartRecording(){
		if(!ReplayKitManager.IsRecording()){
			if(ReplayKitManager.IsPreviewAvailable()){
				ReplayKitManager.Discard();
			}

			ui.SetMode("Recording");
			fc.StartRecPlay();

			#if PLATFORM_ANDROID
				ReplayKitManager.StartRecording(true);
			#else
				ReplayKitManager.StartRecording(false);
			#endif

			isRecording = true;
		}
	}

	public void StopRecording(){
		if(ReplayKitManager.IsRecording()){
			isRecording = false;
			ReplayKitManager.StopRecording();
			ReplayKitManager.Preview();

			ReplayKitManager.SavePreview((error) =>
	        {
	            Debug.Log("Saved preview to gallery with error : " + ((error == null) ? "null" : error));
	        });
	        
			ui.SetMode("Edit");
		}
	}
}
