using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UI_UnlockCircle : MonoBehaviour {

	const string UI_Gauge = "Canvas_UnlockCircle";          //!< UI circle gauge Canvas.
	const string UI_Outer = "Image_Outer";   //!< Slow clockwise on outer cirlce.(24div)
	const string UI_Inner = "Image_Inner";   //!< Fast clockwise on inner circle.(72div)
	const string UI_Numeric = "Image_Numeric";     //!< Count numeric in center.

	GameObject GetObjectFind(string name) {
		GameObject go = GameObject.Find(name);
		if (go == null) {
			Debug.Log("[NOT FOUND] '" + name + "'.");
		}
		return (go);

	}
	GameObject canvas;
	Image Image_Test;
	Image image_Outer;
	Image image_Inner;

	//Outer
	const float cwOuter_cwFillAmount = 1f / 24f;                //!< Normalized value a scale of ruler.
																//Inner															//Outer
	const float cwInner_LapTime_Full360deg = 1f;                //!< Full time of cw 360deg.
	const float cwInner_Time_a_RulerScale = cwInner_LapTime_Full360deg / 72f;   //!< Time of a ruler scale.
	const float cwInner_cwFillAmount = 1f / 72f;                //!< Normalized value a scale of ruler.

	float displayTime;
	GameObject textNumeric;
	Text textNumericTxt;
	int displayCounter;
	float div72timer;
	float div24timer;
	bool unlock = false;
	int unlockCounter;
	bool div72Done = false;

	const int unlockCountSec = 10; //!< Unlock time[sec]
	const float OUTER_DIV24 = 24f;
	float a_div24 = (float)unlockCountSec / OUTER_DIV24;//24 is div in circle.

	void InitialImageType(Image image) {
		image.type = Image.Type.Filled;
		image.fillMethod = Image.FillMethod.Radial360;
		image.fillOrigin = (int)Image.Origin360.Top;//2
	}
	void Start() {
		canvas = GetObjectFind(UI_Gauge);

		//Initial outer
		image_Outer = GetObjectFind(UI_Outer).GetComponent<Image>();
		InitialImageType(image_Outer);
		image_Outer.fillAmount = 0f; // 360deg : 1.0cw

		//Initial inner
		image_Inner = GetObjectFind(UI_Inner).GetComponent<Image>();
		InitialImageType(image_Inner);
		image_Inner.fillAmount = 0f; // 360deg : 1.0cw

		//---
		Image_Test = GetObjectFind("Image_Test").GetComponent<Image>();
		Image_Test.fillAmount = image_Outer.fillAmount;

		//---
		textNumeric = GetObjectFind("/" + UI_Gauge + "/Text_Numeric");
		textNumericTxt = textNumeric.GetComponent<Text>();

		actInactive();//Initial
		actZoomIn();
		InitSound();
	}
	enum actionMode {
		inactive = 0,
		zoomIn,
		active,
		zoomOut
	}
	actionMode actMode = actionMode.inactive;
	const float zoomScaleResolution = 0.05f;//!< Zoom resolution(Zoom speed).
	float zoomScale;

	void init() {
		textNumericTxt.text = displayCounter.ToString("00");
		image_Outer.fillAmount = 0f;
		image_Inner.fillAmount = 0f;
		displayTime = 0f;
		div72timer = 0f;
		displayCounter = 0;
		unlockCounter = 1;
		unlock = false;
	}

	void actZoomOut() {
		actMode = actionMode.zoomOut;
		canvas.transform.localScale = new Vector3(0, 0, 0);

	}
	void actZoomIn() {
		init();
		actMode = actionMode.zoomIn;
		zoomScale = 0;
		canvas.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
	}
	void actActive() {
		actMode = actionMode.active;
		zoomScale = 1;
		canvas.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
	}
	void actInactive(){
		init();
		actMode = actionMode.inactive;
		zoomScale = 0;
		canvas.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
	}
	float t = 0;
	void Update() {
		DebugModeCon(); //Make debug trigger
		//-----
		switch (actMode) {
			case actionMode.zoomIn:
				zoomScale += zoomScaleResolution;
				if (zoomScale > 1.0f) {
					zoomScale = 1.0f;
					audioSource_opupUI01.Play();
					actActive();
				}
				canvas.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);
				break;
			case actionMode.active:
				if (unlock == false) {
					div72timer += Time.deltaTime;
					if (div72timer > 1.0f) {
						div72timer -= 1.0f;
						displayCounter++;
						div72Done = true;
					}
					else {
						div72Done = false;
					}
					image_Inner.fillAmount = div72timer;

					//Outer
					div24timer += Time.deltaTime;
					if (div24timer > a_div24) {
						div24timer -= a_div24;
						unlockCounter++;
						image_Outer.fillAmount = (float)unlockCounter / OUTER_DIV24;
					}

					displayTime += Time.deltaTime;
					textNumericTxt.text = displayCounter.ToString("00");

					if ((unlockCounter > OUTER_DIV24)) {
						if ((div72Done == true)) {
							unlock = true;
							audioSource_opupUI01.Play();
						}
					}
				}
				break;
			case actionMode.zoomOut:
				zoomScale -= zoomScaleResolution;
				if (zoomScale < 0.0f) {
					zoomScale = 0.0f;
					actInactive();
				}
				canvas.transform.localScale = new Vector3(zoomScale, zoomScale, zoomScale);

				break;
			default:
				break;
		}
	}
	AudioSource audioSource_opupUI01;
	void InitSound() {
		const string SNDNAME_PopupUI01 = "push59_c";
		AudioClip audioClip_PopupUI01;
		audioClip_PopupUI01 = Resources.Load<AudioClip>(SNDNAME_PopupUI01);
		audioSource_opupUI01 = gameObject.AddComponent<AudioSource>();

		audioSource_opupUI01.clip = audioClip_PopupUI01; //音色(wav)をチャンネルに紐付け
		audioSource_opupUI01.volume = 1.0f;  //ボリューム設定。0～1.0範囲
	}



	//Begin: Make debug trigger
	void DebugModeCon() {
		t += Time.deltaTime;
		switch (actMode) {
			case actionMode.zoomIn:
				t = 0;
				break;
			case actionMode.active:
				if (unlock == true) {
					if (t > (float)unlockCountSec + 1) {
						actZoomOut();
						t = 0;
					}
				}
				break;
			case actionMode.zoomOut:
				t = 0;
				break;
			case actionMode.inactive:
				if (t > 2) {
					actZoomIn();
					t = 0;
				}
				break;
			default:
				break;

		}
	}
	//End: Make debug trigger
}
