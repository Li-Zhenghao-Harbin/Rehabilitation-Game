using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardItems : MonoBehaviour
{

    // GameObject
    public const int maxCardItemCount = 5;
    public GameObject[] cardItem = new GameObject[maxCardItemCount];
    public bool[] isCardItemActive = new bool[maxCardItemCount];
    const float cardItemGeneratedHeight = 3f;
    // Plane
    const float planeSideLength = 4f;

    // Use this for initialization
    void Start()
    {
        // Set GameObjects
        for (int i = 0; i < 5; i++)
        {
            cardItem[i] = GameObject.Find("CardItem" + (i + 1).ToString());
        }
    }

    public void InactiveCardItem(int index)
    {
        var v = cardItem[index].transform.position;
        v.x = v.z = -100;
        cardItem[index].transform.position = v;
        isCardItemActive[index] = false;
    }

    public void ActiveCardItem()
    {
        for (int i = 0; i < maxCardItemCount; i++)
        {
            if (isCardItemActive[i] == false)
            {
                isCardItemActive[i] = true;
                var v = cardItem[i].transform.position;
                v.x = Random.Range(-planeSideLength, planeSideLength + 1);
                v.z = Random.Range(-planeSideLength, planeSideLength + 1);
                v.y = cardItemGeneratedHeight;
                cardItem[i].transform.position = v;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < maxCardItemCount; i++)
        {
            if (cardItem[i].transform.position.y < -100)
            {
                isCardItemActive[i] = false;
            }
        }
    }
}
