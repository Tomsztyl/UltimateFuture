using System.Collections;
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
    [SerializeField] private Vector3 dropObjectPostion = new Vector3(0f, 2f, 4f);
    private Vector3 tranformObjectCalcuated;

    private void Start()
    {
        SetTextCount();
    }

    #region Mechanics Drop Object
    public void TriggerDropObject()
    {
        if (_currentSlot!=null)
        {
            CheckIsSlotEmpty();

            if (_sliderCount.value!=0&& !CheckIsSlotEmpty())
            {
                //Drop Object Mechanism
                _currentSlot.SetCountObjectSubstract(_sliderCount.value);
                _currentSlot.SetPropertiesDrop();
                DropObjectFromCharacter();
                CheckIsSlotEmpty();
            }
            
        }
    }
    private bool CheckIsSlotEmpty()
    {
        if (_currentSlot.ReturnCountObject() <= 0 || _currentSlot.ReturnSprite() == null || _currentSlot.ReturnPrefab() == null)
        {
            SetMaxValueSlider(0);
            SetImageDrop(null, false);
            return true;
        }
        return false;
    }
    private void DropObjectFromCharacter()
    {
        PlayerMirrorController playerMirrorController = this.gameObject.transform.root.GetComponent<PlayerMirrorController>();

        if (playerMirrorController!=null)
        {
            if (_currentSlot.ReturnPrefab()!=null)
            {
                playerMirrorController.DropObjectServer(_currentSlot.ReturnPrefab().name,TranfromDropObjectFromCharacter(), _sliderCount.value);
            }
        }
    }
    private Vector3 TranfromDropObjectFromCharacter()
    {
        CalculateInstantiateObjectDrop();
        return tranformObjectCalcuated;
    }
    private void CalculateInstantiateObjectDrop()
    {
        tranformObjectCalcuated = new Vector3(transform.root.position.x, transform.root.position.y + dropObjectPostion.y, transform.root.position.z) + (transform.root.forward * dropObjectPostion.z);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        CalculateInstantiateObjectDrop();
        Gizmos.DrawWireCube(tranformObjectCalcuated, new Vector3(2f,2f,2f));
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
    public void SetImageDrop(Sprite spriteDropObject, bool imageActiveDrop)
    {
        _imageDrop.sprite = spriteDropObject;
        _imageDrop.enabled = imageActiveDrop;
    }
    public void SetCurrentSlot(SlotController slotController)
    {
        _currentSlot = slotController;
    }
    #endregion
}
