using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;
using VRC.Udon;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Slimothy : UdonSharpBehaviour
{
    [Header("--- Slimothy Controller --- ")]
    public Animator SlimothyAnimation;
    public Collider SlimothyCollider;
    [Header("--- Slimothy Text ---")]
    public TMP_Text slimothyText;
    public GameObject textObject;
    [UdonSynced] private string lastPlayerWhoSquished;
    [UdonSynced] private string currentText; 
    [UdonSynced] private bool squishing;
    [UdonSynced] private bool sad;
    [UdonSynced] private bool idle;

    public override void OnPlayerTriggerEnter(VRCPlayerApi p){
        lastPlayerWhoSquished = p.displayName;
        Networking.SetOwner(Networking.LocalPlayer, textObject);
        SendCustomNetworkEvent(NetworkEventTarget.All, nameof(SlimothySquishHandler));
        RequestSerialization();
        SlimothySquishHandler();
    }

    public override void OnDeserialization(){
        SlimothySquishHandler();
    }

    private void SlimothySquishHandler(){
        // If he is not getting squished
        if(!squishing){
            squishing = true;
            sad = false;
            idle = false;
            currentText = "Weh >-<";
            slimothyText.text = currentText;
            if(!textObject.activeSelf) textObject.SetActive(true);
            SlimothyAnimation.Play("BaseLayer.Squishing");
        }
    }

    public void Update(){
        slimothyText.text = currentText;
        
        // If idle change sad state to false
        if(idle){
            sad = false;
            textObject.SetActive(false);
        }
        // While sad keep checking for if idle
        else if(sad){
            idle = SlimothyAnimation.GetCurrentAnimatorStateInfo(0).IsName("BaseLayer.Idle");
            if(squishing) squishing = false;
            currentText = "I hate you\n "+lastPlayerWhoSquished+"!!!";
        }
        // Not Idle or Sad so Squished, check for sad
        else if(!sad){
            sad = SlimothyAnimation.GetCurrentAnimatorStateInfo(0).IsName("BaseLayer.Sad");
        }
    }
}
