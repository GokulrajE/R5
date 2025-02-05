using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSceneController : MonoBehaviour
{
    public TMP_Text patientNameText;
    public TMP_Text patientHospNoText;

    void Start()
    {
        string[] patientInfo = Staticvlass.CrossSceneInformation.Split(',');
        if (patientInfo.Length == 2)
        {
            //patientNameText.text = "Name: " + patientInfo[0];
            //patientHospNoText.text = "Hospital No: " + patientInfo[1];
            patientNameText.text =  patientInfo[0];
            patientHospNoText.text =   patientInfo[1];
        }
    }
}
