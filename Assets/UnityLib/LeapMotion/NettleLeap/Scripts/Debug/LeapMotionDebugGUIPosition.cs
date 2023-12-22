using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nettle.NettleLeap;
using UnityEngine.UI;

public class LeapMotionDebugGUIPosition : MonoBehaviour
{
    private const string _positionLabelText = "NettleLeap GUI position: ";    
    private const string _widthLabelText = "Screen width: ";
    [SerializeField]
    private LeapMotionGUIManager _guiManager;
    [SerializeField]
    private Text _positionLabel;
    [SerializeField]
    private Text _widthLabel;
    [SerializeField]
    private float _valueStep = 0.05f;
    [Header("Hotkeys")]
    [SerializeField]
    private KeyCode _xPlusHotkey = KeyCode.Keypad7;    
    [SerializeField]
    private KeyCode _xMinusHotkey = KeyCode.Keypad4;
    [SerializeField]
    private KeyCode _yPlusHotkey = KeyCode.Keypad8;    
    [SerializeField]
    private KeyCode _yMinusHotkey = KeyCode.Keypad5;    
    [SerializeField]
    private KeyCode _zPlusHotkey = KeyCode.Keypad9;    
    [SerializeField]
    private KeyCode _zMinusHotkey = KeyCode.Keypad6;
    [SerializeField]
    private KeyCode _widthPlusHotkey = KeyCode.Keypad1;
    [SerializeField]
    private KeyCode _widthMinusHotkey = KeyCode.Keypad2;

    private void Update()
    {
        bool anyChange = false;
        if (Input.GetKeyDown(_xPlusHotkey)){
            _guiManager.ControllerShift.x += _valueStep;
            anyChange = true;
        }
        if (Input.GetKeyDown(_xMinusHotkey)){
            _guiManager.ControllerShift.x -= _valueStep;
            anyChange = true;
        }

        if (Input.GetKeyDown(_yPlusHotkey)){
            _guiManager.ControllerShift.y += _valueStep;
            anyChange = true;
        }
        if (Input.GetKeyDown(_yMinusHotkey)){
            _guiManager.ControllerShift.y -= _valueStep;
            anyChange = true;
        }

        if (Input.GetKeyDown(_zPlusHotkey)){
            _guiManager.ControllerShift.z += _valueStep;
            anyChange = true;
        }
        if (Input.GetKeyDown(_zMinusHotkey)){
            _guiManager.ControllerShift.z -= _valueStep;
            anyChange = true;
        }
                
        if (Input.GetKeyDown(_widthPlusHotkey)){
            _guiManager.ScreenWidth += _valueStep;
            anyChange = true;
        }
        if (Input.GetKeyDown(_widthMinusHotkey)){
            _guiManager.ScreenWidth -= _valueStep;
            anyChange = true;
        }        
        _positionLabel.text = _positionLabelText + _guiManager.ControllerShift.ToString("F2");
        _widthLabel.text = _widthLabelText + _guiManager.ScreenWidth.ToString("F2");
        if (anyChange)
        {
            _guiManager.UpdateRootPosition();
        }
    }



}
