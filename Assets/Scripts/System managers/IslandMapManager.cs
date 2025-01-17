﻿using System.Collections;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine.SceneManagement;

public class IslandMapManager : MonoBehaviour
{
    [SerializeField]
    Camera camera;
    [SerializeField]
    AttackIsland attackIsland;
    [SerializeField]
    int islandIDPlayer;
    Vector3 playerIsland;
    //Enums for results of battle
    enum resultsBattle { AlreadyInBattle = 0, Success = 1 }

    private int islandID; 
    private long timeOfAttack = 0;
    private string timeOfAttackDHMS;
    [SerializeField]
    private TMPro.TextMeshProUGUI timeLeftTxt, islandIDTxt;
    [SerializeField]
    GameObject attackIslandPanel, timerPanel, returnToIslandPanel;
    [SerializeField]
    Canvas canvas;
    void Start()
    {
        //Get your island ID so we know what island to center on
        islandIDPlayer = PlayerPrefs.GetInt("ISLANDID");
        timeLeftTxt.text = "";
        attackIslandPanel.SetActive(false);
        timerPanel.SetActive(false);
        canvas.enabled = true;

        //Check if we got the players island position saved
        if (!PlayerPrefs.HasKey("ISLANDTRANSFORM_X"))
        {
            //Find the position of player island
            FindPlayerIsland();
        } 

        //Save set the position of player island
        float x = PlayerPrefs.GetFloat("ISLANDTRANSFORM_X");
        float z = PlayerPrefs.GetFloat("ISLANDTRANSFORM_Z");
        playerIsland = new Vector3(x, 750, z);
        camera.transform.position = playerIsland;


        camera.GetComponent<CameraControllMapView>().playerIsland = playerIsland;
        //Get attack time so we get the time once you start up the island map
        attackIsland.GetAttackTime(OnGetAttackTime);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            //Check if we are over a not over a UI element and 
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                //Check if we hit an island and that it's not player island
                if (hit.collider.tag == "Island" && hit.collider.name != "Island" + islandIDPlayer)
                {
                    //Show attack UI
                    islandID = hit.collider.gameObject.GetComponent<IslandOwner>().islandID;
                    islandIDTxt.text = "Island: " + islandID.ToString();
                    returnToIslandPanel.SetActive(false);
                    attackIslandPanel.SetActive(true);
                } else if(hit.collider.name == "Island" + islandIDPlayer)
                {
                    //Show return home UI
                    attackIslandPanel.SetActive(false);
                    returnToIslandPanel.SetActive(true);
                }
            }
        }
    }


    public void AttackPlayer()
    {
        //Set disable panel to give the user a hint that the button worked
        attackIslandPanel.SetActive(false);
        timerPanel.SetActive(true);
        timeLeftTxt.text = "Creating attack plan";
        attackIsland.AttackPlayer(islandID, OnAttack);
    }

    public void CloseAttackPanel()
    {
        attackIslandPanel.SetActive(false);
    }

    public void CancleAttack()
    {
        attackIsland.CancleAttack(OnAttackCancelled);
    
    }

    void OnAttackCancelled(ExecuteCloudScriptResult result)
    {
        attackIsland.GetAttackTime(OnGetAttackTime);
    }

    //Gets the result of the calcAttackTime cloudscript
    //Checks if value is an error or not and exectute next script
    void OnAttack(ExecuteCloudScriptResult result)
    {

        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object canAttack;
        jsonResult.TryGetValue("result", out canAttack);

        Debug.Log(canAttack);

        if (System.Convert.ToInt32(canAttack) == (int)resultsBattle.Success)
        {
            attackIsland.GetAttackTime(OnGetAttackTime);
        }
    }




    //Gets the results of checkBattle cloudscript (UNIX time in millis until battle)
    void OnGetAttackTime(ExecuteCloudScriptResult result)
    {
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object timeOfAttackObject;
        jsonResult.TryGetValue("timeOfAttack", out timeOfAttackObject);

        Debug.Log(timeOfAttack);

        if(timeOfAttackObject.ToString() == "noBattle")
        {
            Debug.Log("Work it");
            return;
        }

        if (timeOfAttackObject.ToString() == "cancelled")
        {
            timerPanel.SetActive(true);
            timeLeftTxt.text = "Attack cancelled";
            StopCoroutine("updateTime");
        }
        else
        {
            timerPanel.SetActive(true);
            //Convert object to long
            timeOfAttack = System.Convert.ToInt64(timeOfAttackObject);
            StartCoroutine("updateTime");
        }
    }


    //Gets the result of the battle and displays it on screen
    void OnGetResult(ExecuteCloudScriptResult result)
    {
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object resultOfBattle;
        jsonResult.TryGetValue("result", out resultOfBattle);

        timerPanel.SetActive(true);
        timeLeftTxt.text = resultOfBattle.ToString();
    }



    //Count down the amount of time left before attack is over
    IEnumerator updateTime()
    {
        while (true)
        {

            timeOfAttack -= 1000;

            if (timeOfAttack > 0)
            {

                var diff = timeOfAttack / 1000;
                var secondsDiff = diff % 60;
                diff = diff / 60;
                var minutesDiff = diff % 60;
                diff = diff / 60;
                var hoursDiff = diff % 24;
                diff = diff / 24;
                var daysDiff = diff % 7;

                timeOfAttackDHMS = daysDiff.ToString() + ":" + hoursDiff.ToString() + ":" + minutesDiff.ToString() + ":" + secondsDiff.ToString();

                timeLeftTxt.text = timeOfAttackDHMS;

                yield return new WaitForSeconds(1f);
            }
            else
            {

                timeLeftTxt.text = "";
                timerPanel.SetActive(false);
                attackIsland.CalculateWinner(OnGetResult);
                break;
            };

        }
    }


    void FindPlayerIsland()
    {
        playerIsland = GameObject.Find("Island" + islandIDPlayer).transform.position;

        PlayerPrefs.SetFloat("ISLANDTRANSFORM_X", playerIsland.x);
        PlayerPrefs.SetFloat("ISLANDTRANSFORM_Z", playerIsland.z);

    }


    public void ReturnToIsland()
    {
        SceneManager.LoadScene("PrivateIsland");
    }

    public void CloseReturnToIslandPanel()
    {
        returnToIslandPanel.SetActive(false);
    }

  

}
