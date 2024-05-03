using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    public void ChangeScene(string s){
        SceneManager.LoadScene(s);
    }

    void Start(){
        if(text!=null){
            int end = PlayerPrefs.GetInt("end",0);
            if(end==0){
                text.text = "THE MONSTER GOT YOU";
            }
            else if (end==1){
                text.text = "YOU GOT TO THE RADIO";
            }
        }
    }
}
