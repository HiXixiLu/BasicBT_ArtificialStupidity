using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachineUI : MonoBehaviour
{
    public Text citizen;
    public Text destroyer;
    public Text police;

    void resetTextColor() {
        citizen.color = Color.black;
        police.color = Color.black;
        destroyer.color = Color.black;
    }
    public void onCitizenRound() {
        resetTextColor();
        citizen.color = Color.green;
    }
    public void onPoliceRound() {
        resetTextColor();
        police.color = Color.green;
    }
    public void onDestroyerRound() {
        resetTextColor();
        destroyer.color = Color.green;
    }
}
