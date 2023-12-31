/******************************************************************************
 * Copyright (C) Leap Motion, Inc. 2011-2017.                                 *
 * Leap Motion proprietary and  confidential.                                 *
 *                                                                            *
 * Use subject to the terms of the Leap Motion SDK Agreement available at     *
 * https://developer.leapmotion.com/sdk_agreement, or another agreement       *
 * between Leap Motion and you, your company or other organization.           *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using Leap;
using System.Collections;

namespace Leap.Unity{
  public class TemporalWarpingStatus : MonoBehaviour {
    public LeapVRTemporalWarping cameraAlignment;
  
    protected Text textField;
  
    protected SmoothedFloat _imageLatency = new SmoothedFloat();
    protected SmoothedFloat _frameDelta = new SmoothedFloat();
    [SerializeField]
    LeapProvider Provider;
  
    void Start () {
      textField = GetComponent<Text> ();
      if (textField == null) {
        gameObject.SetActive(false);
      }
  
      _imageLatency.delay = 0.1f;
      _frameDelta.delay = 0.1f;
    }
  	
  	// Update is called once per frame
    void Update () {
      //if (cameraAlignment == null) {
      //  Debug.Log("TemporalWarpingStatus requires LeapCameraAlignment reference -> status will be disabled");
      //  gameObject.SetActive(false);
      //  return;
      //}
  
      //if (!cameraAlignment.isActiveAndEnabled) {
      //  return;
      //}
  
      //ImageList list = Provider.CurrentFrame.Images;
      //  Leap.Image image = list.IRLeft;
      //    float latency = Provider.GetLeapController().Now() - image.Timestamp;
      //    _imageLatency.Update(latency, Time.unscaledDeltaTime);
  
  
      //_frameDelta.Update(Time.unscaledDeltaTime, Time.unscaledDeltaTime);
  
      //string statusText = "IMAGE LATENCY: " + (_imageLatency.value / 1000f).ToString("#00.0") + " ms\n";
      //statusText += "FRAME DELTA: " + (_frameDelta.value * 1000).ToString ("#00.0") + " ms\n";
      //statusText += "REWIND ADJUST: " + (cameraAlignment.RewindAdjust).ToString ("#00.0") + " ms\n";
  
      //textField.text = statusText;
  	}
  }
}
