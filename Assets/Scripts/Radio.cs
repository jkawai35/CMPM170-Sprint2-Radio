using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Radio : MonoBehaviour
{

    public static Radio _instance;
    public static Radio Instance {get{return _instance;}}
    AudioSource audioSource;

    [SerializeField] Image cooldownImg;
    float cooldown = 0;
    public bool interference = false;
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
                energyBar.color = Color.green;
                energyBar.fillAmount = currentEnergy/maxEnergy;
                if(currentEnergy==0){
                    currentState = RadioState.isOff;
                }
            break;
            case RadioState.isCharging:
                currentEnergy += drainSpeed * 2 * Time.deltaTime;
                currentEnergy = Mathf.Clamp(currentEnergy,0,maxEnergy);
                energyBar.color = Color.cyan;
                energyBar.fillAmount = currentEnergy/maxEnergy;
                if(currentEnergy==maxEnergy){
                    currentState = RadioState.isOn;
                }
            break;
        }
        cooldown += 0.2f*Time.deltaTime;
        cooldownImg.fillAmount = cooldown;
    }

    public void RadioDirection(int x){
        if(currentEnergy>0){
            cooldown = 0;
            if(!interference){
                audioSource.PlayOneShot(soundList[x]);
            }
            else{
                int rand = Random.Range(0,3);
                audioSource.PlayOneShot(soundList[rand]);
            }
            
        }
    }
}
