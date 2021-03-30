using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
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


    [Header("This is a controls to Bar Controller")]
    [Tooltip("This is a object from HUD Player to Control Healt")]
    [SerializeField] private GameObject barControllerLeftHealt = null;
    [SerializeField] private TextMeshProUGUI textMeshProUGUIHealt=null;
    HealthController health;
    private Material mat;
    public float fillTarget = .5f;
    public float delta = 0f;
    public float dampening = 5f;






    private void Start()
    {
        FindObjectMateria();
    }

    private void Update()
    {
        EnableConsole();


    }
    private void LateUpdate()
    {
        ChangeToolBoxActive();

        if (health != null)
        {
            ControllBarHealth(health.healt, health.healtdef);
        }
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
    #region Mechanics Bar Health
    public void ControllBarHealth(float health,float defValue)
    {
        //Debug.Log(PercentToValue(1, ValueToPercent(health, defValue)));
        delta -= fillTarget - PercentToValue(1, ValueToPercent(health, defValue));
        fillTarget = PercentToValue(1, ValueToPercent(health, defValue));

        if (textMeshProUGUIHealt != null) {textMeshProUGUIHealt.text =ValueToPercent(health, defValue) + "%"; }

        delta = Mathf.Lerp(delta, 0, Time.deltaTime * dampening);

        mat.SetFloat("_Delta", delta);
        mat.SetFloat("_Fill", fillTarget);
    }
    private void FindObjectMateria()
    {
        health = this.gameObject.transform.root.GetComponent<HealthController>();

        Renderer rend = barControllerLeftHealt.GetComponent<Renderer>();
        Image img = barControllerLeftHealt.GetComponent<Image>();
        if (rend != null)
        {
            mat = new Material(rend.material);
            rend.material = mat;
        }
        else if (img != null)
        {
            mat = new Material(img.material);
            img.material = mat;
        }
        else
        {
            Debug.LogWarning("No Renderer or Image attached to " + name);
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
    #endregion
}
