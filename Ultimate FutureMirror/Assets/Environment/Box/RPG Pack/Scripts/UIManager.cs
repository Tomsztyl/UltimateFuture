using Mirror;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    //Get Properties Kind Bar
    public enum KindBarHUD
    {
        health,
        stamine,
    }
    //Properties needed for Bar HUD
    HealthController health;
    PlayerController playerController;


    [Header("This is Message Panel")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private Text messagePanelTxt;

    [Header("This is a properties to Console")]
    //Variable Controller Console
    [SerializeField] private GameObject console;
    [SerializeField] private KeyCode keyControlConsole = KeyCode.BackQuote;

    [Header("This is a controls Equipment")]
    //Variable SelectBoxEq
    [SerializeField] private GameObject toolBox;
    [SerializeField] private KeyCode keyControlToolBox = KeyCode.E;
    bool activeToolBox = false;
    

    private void Start()
    {
        FindObjectsRequiredBar();
    }

    private void Update()
    {
        EnableConsole();
    }
    private void LateUpdate()
    {
        ChangeToolBoxActive();
        ChooseBarMechanic();
    }


    public void ManagerMessagePanel(string textMessagePanel, bool isInColider)
    {
        if (isInColider==true)
        {
            messagePanel.SetActive(true);
            messagePanelTxt.text = textMessagePanel;
        }
        else
        {
            messagePanel.SetActive(false);
            messagePanelTxt.text = textMessagePanel;
        }
    }
    private void EnableConsole()
    {
        if (Input.GetKeyDown(keyControlConsole))
        {
            if (console.active == false)
            {
                Screen.lockCursor = false;
                Cursor.visible = true;
                console.SetActive(true);
            }
            else
            {
                Screen.lockCursor = true;
                Cursor.visible = false;
                console.SetActive(false);
            }
        }
    }
    private void ChangeToolBoxActive()
    {

        if (Input.GetKeyDown(keyControlToolBox) && activeToolBox == false)
        {
            toolBox.SetActive(true);
            activeToolBox = true;
        }
        else if (Input.GetKeyDown(keyControlToolBox)&&activeToolBox==true)
        {
            toolBox.SetActive(false);
            activeToolBox = false;
        }
    }
    #region Mechanics Bar 
    private void FindObjectsRequiredBar()
    {
        health = this.gameObject.transform.root.GetComponent<HealthController>();
        playerController = this.gameObject.transform.root.GetComponent<PlayerController>();
        SetPropertiesMaterial(KindBarHUD.health, FindObjectMateria(GetBarPropetriesHUD(KindBarHUD.health).barController));
        SetPropertiesMaterial(KindBarHUD.stamine, FindObjectMateria(GetBarPropetriesHUD(KindBarHUD.stamine).barController));
    }
    private void ChooseBarMechanic()
    {
        if (health != null && GetBarPropetriesHUD(KindBarHUD.health).barMaterial != null)
        {
            ControllBarHealth(KindBarHUD.health, health.healt, health.healtdef, GetBarPropetriesHUD(KindBarHUD.health).barMaterial, GetBarPropetriesHUD(KindBarHUD.health).textMeshProUGUI);
        }

        if (playerController != null && GetBarPropetriesHUD(KindBarHUD.stamine).barMaterial != null)
        {
            ControllBarStamine(KindBarHUD.stamine, playerController.GetCalculateStamine(), playerController.GetStamineTime(), GetBarPropetriesHUD(KindBarHUD.stamine).barMaterial, GetBarPropetriesHUD(KindBarHUD.stamine).textMeshProUGUI);
        }
    }
    private void ControllBarHealth(KindBarHUD kindBarHUD, float health, float defHealth, Material materialHealthBar, TextMeshProUGUI textProBarHealth)
    {
        ControllBar(kindBarHUD, health, defHealth, materialHealthBar, textProBarHealth);
    }
    private void ControllBarStamine(KindBarHUD kindBarHUD, float stamine, float deStamine, Material materialStamineBar, TextMeshProUGUI textProBarHealth)
    {
        ControllBar(kindBarHUD, stamine, deStamine, materialStamineBar, textProBarHealth);
    }
    private void ControllBar(KindBarHUD kindBarHUD,float value, float defValue, Material materialBar, TextMeshProUGUI textProBar)
    {
        float fillTarget = GetBarPropetriesHUD(kindBarHUD).fillTarget;
        float delta= GetBarPropetriesHUD(kindBarHUD).delta;

        delta -= GetBarPropetriesHUD(kindBarHUD).fillTarget - PercentToValue(1, ValueToPercent(value, defValue));
        fillTarget = PercentToValue(1, ValueToPercent(value, defValue));


        if (textProBar != null) { textProBar.text = ValueToPercent(value, defValue) + "%"; }

        delta = Mathf.Lerp(delta, 0, Time.deltaTime * GetBarPropetriesHUD(kindBarHUD).dampening);

        SetPropertiesDelta(kindBarHUD, delta);
        SetPropertiesFillTarget(kindBarHUD, fillTarget);

        materialBar.SetFloat("_Delta", GetBarPropetriesHUD(kindBarHUD).delta);
        materialBar.SetFloat("_Fill", GetBarPropetriesHUD(kindBarHUD).fillTarget);
    }
    public Material FindObjectMateria(GameObject objectBar)
    {
        Material materialObject;


        Renderer rend = objectBar.GetComponent<Renderer>();
        Image img = objectBar.GetComponent<Image>();
        if (rend != null)
        {
            materialObject = new Material(rend.material);
            rend.material = materialObject;

            return materialObject;
        }
        else if (img != null)
        {
            materialObject = new Material(img.material);
            img.material = materialObject;

            return materialObject;
        }
        else
        {
            Debug.LogWarning("No Renderer or Image attached to " + name);
            return null;
        }
    }
    private int ValueToPercent(float value,float defValue)
    {
        return (int)((int)value * 100 / defValue);
    }
    private float PercentToValue(float value,int percent)
    {
        return (float)((float)value * percent * 0.01);
    }
    #region Properties Bar
    [SerializeField]
    public BarPropertiesHUD _BarPropertiesHUD;

    [System.Serializable]
    public class BarPropertiesHUD : SerializableDictionaryBase<KindBarHUD, BarPropertie> { }

    [System.Serializable]
    public class BarPropertie
    {
        public GameObject barController = null;
        public TextMeshProUGUI textMeshProUGUI = null;
        public Material barMaterial = null;
        public float fillTarget = .5f;
        public float delta = 0f;
        public float dampening = 5f;
    }

    public BarPropertie GetBarPropetriesHUD(KindBarHUD kindBarHUD)
    {
        return _BarPropertiesHUD.FirstOrDefault(x => x.Key == kindBarHUD).Value;
    }
    public void SetPropertiesMaterial(KindBarHUD kindBarHUD, Material materialBar)
    {
        _BarPropertiesHUD.FirstOrDefault(x => x.Key == kindBarHUD).Value.barMaterial= materialBar;
    }
    public void SetPropertiesDelta(KindBarHUD kindBarHUD, float delta)
    {
        _BarPropertiesHUD.FirstOrDefault(x => x.Key == kindBarHUD).Value.delta= delta;
    }
    public void SetPropertiesFillTarget(KindBarHUD kindBarHUD, float fillTarget)
    {
        _BarPropertiesHUD.FirstOrDefault(x => x.Key == kindBarHUD).Value.fillTarget = fillTarget;
    }

    #endregion
    #endregion
}
