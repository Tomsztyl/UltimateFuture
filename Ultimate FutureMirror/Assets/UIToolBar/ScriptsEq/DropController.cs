﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropController : MonoBehaviour
{
    [Header("This is a display for Drop Aciton")]
    [SerializeField] private Text _textCount = null;
    [SerializeField] private Slider _sliderCount = null;
    [SerializeField] private Button _buttonDrop = null;
    [SerializeField] private Image _imageDrop = null;

    [Header("This is a variable to drop object")]
    [SerializeField] private SlotController _currentSlot = null;

    private void Start()
    {
        SetTextCount();
    }

    #region Mechanics Drop Object
    public void TriggerDropObject()
    {
        if (_currentSlot!=null)
        {
            _currentSlot.SetCountObjectSubstract(_sliderCount.value);
            _currentSlot.SetPropertiesDrop();
        }
    }
    #endregion

    #region Set Properties Controller
    public void SetTextCount()
    {
        _textCount.text = "" + _sliderCount.value;
    }
    public void SetMaxValueSlider(float valueMax)
    {
        _sliderCount.maxValue = valueMax;
    }
    public void SetImageDrop(Sprite spriteDropObject)
    {
        _imageDrop.sprite = spriteDropObject;
        _imageDrop.enabled = true;
    }
    public void SetCurrentSlot(SlotController slotController)
    {
        _currentSlot = slotController;
    }
    #endregion
}
