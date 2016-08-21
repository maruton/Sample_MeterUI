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
	const float cwInner_LapTime_Full360deg = 1f;                        //!< Full time of cw 360deg.
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

	const int unlockCountSec = 6;//[sec]
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

		GauseSetup();
	}
	void GauseSetup() {
		displayTime = 0f;
		div72timer = 0f;
		displayCounter = 0;
		unlockCounter = 1;
		unlock = false;

		canvas.transform.localScale = new Vector3(0,0,0);
	}
	bool active = false;
	void GauseStart()
	{
		unlock = false;
		active = true;
		canvas.transform.localScale = new Vector3(1, 1, 1);
	}
	float t = 0;
	void Update () {
		//debug
		if (active == false) {
			t += Time.deltaTime;
			if (t > 5) GauseStart();
		}
		//

		if (active == false) return;
		if (unlock == true) return;


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
		else {
		}

		
		displayTime += Time.deltaTime;
		textNumericTxt.text = displayCounter.ToString("00");

		if ((unlockCounter > OUTER_DIV24) && (div72Done==true)) {
			unlock = true;
		}
	}
}
