using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numAngryText;
    [SerializeField] private int loseNumAngryCustomers = 8;

    public static CustomerManager instance;

    private int numAngryCustomers;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void setAngryCustomer(bool add)
    {
        numAngryCustomers += add ? 1 : -1;

        numAngryText.text = numAngryCustomers + "/" + loseNumAngryCustomers;

        if (numAngryCustomers >= loseNumAngryCustomers)
            lose();
    }

    void lose()
    {
        Debug.Log("GAME OVER");
    }
}
