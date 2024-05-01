using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radio : MonoBehaviour
{

    public static Radio _instance;
    public static Radio Instance {get{return _instance;}}
    AudioSource audioSource;
    private void Awake(){
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
        }
        else{
            _instance =  this;
        }
        audioSource = GetComponent<AudioSource>();
    }

    [SerializeField] Image energyBar;

    public float maxEnergy;
    float currentEnergy;
    public float drainSpeed;
    public enum RadioState{
        isOn,
        isOff,
        isCharging
    }

    [SerializeField] List<AudioClip> soundList = new List<AudioClip>();

    public RadioState currentState;
    void Start(){
        currentEnergy = maxEnergy;
        currentState = RadioState.isOn;
    }

    void Update(){
        switch(currentState){
            case RadioState.isOn:
                currentEnergy -= drainSpeed * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy,0,maxEnergy);
                energyBar.fillAmount = currentEnergy/maxEnergy;
                if(currentEnergy==0){
                    currentState = RadioState.isOff;
                }
            break;
            case RadioState.isCharging:
                currentEnergy += drainSpeed * 2 * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy,0,maxEnergy);
                energyBar.fillAmount = currentEnergy/maxEnergy;
                if(currentEnergy==maxEnergy){
                    currentState = RadioState.isOn;
                }
            break;
        }
    }

    public void RadioDirection(int x){
        switch(currentState){
            case RadioState.isOn:
                audioSource.PlayOneShot(soundList[x]);
            break;
        }
    }
}
