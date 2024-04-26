using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radio : MonoBehaviour
{
    [SerializeField] Image energyBar;
    public float maxEnergy;
    float currentEnergy;
    public float drainSpeed;
    public bool isOn = true;
    void Start(){
        currentEnergy = maxEnergy;
    }

    void Update(){
        if(isOn){
            currentEnergy -= drainSpeed * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy,0,maxEnergy);
            energyBar.fillAmount = currentEnergy/maxEnergy;
            if(currentEnergy==0){
                isOn = false;
            }
        }
    }
}
