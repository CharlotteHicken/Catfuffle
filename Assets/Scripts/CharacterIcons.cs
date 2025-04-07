using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterIcons : MonoBehaviour
{
    // Start is called before the first frame update
    public Image playerIcon;
    public Sprite healthyIcon;
    public Sprite knockOutIcon;
    public Sprite eliminatedIcon;
    
    bool isKnockedOut;
    bool isAlive;
    public PlayerController player;

    // Update is called once per frame
    private void Start()
    {
        
    }
    void Update()
    {
        isAlive = player.catBody.activeInHierarchy.Equals(true);
        if(player.hitCount>=player.maxHitCount)
        {
            isKnockedOut = true;
        }
        Debug.Log("THEE ELIMINATION: "+isAlive);
        ChangeIcon();
        
    }
   void ChangeIcon()
    {

        if (isAlive == false)  
        {
            playerIcon.sprite = eliminatedIcon;
        }
        else if(isKnockedOut==true)
        {
            playerIcon.sprite = knockOutIcon;
        }
        else
        {
            playerIcon.sprite = healthyIcon;
            isKnockedOut = false;
        }
    }
}
