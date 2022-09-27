using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numAngryText;
    [SerializeField] private int loseNumAngryCustomers = 8;

    public static CustomerManager instance;
    [SerializeField] int numCustomers;

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

        if (shouldLose())
            lose();
    }

    public bool shouldLose()
    {
        return numAngryCustomers >= loseNumAngryCustomers;
    }

    void lose()
    {
        Debug.Log("GAME OVER");
        MenuManager.instance.lose();
    }

    public int getNumCustomers()
    {
        return numCustomers;
    }

    public void addCustomer()
    {
        numCustomers++;

        changeNumCustomers();
    }
    public void removeCustomer(Customer customer)
    {
        Debug.Log("destroying customer");
        Destroy(customer.gameObject);
        numCustomers--;

        changeNumCustomers();
    }

    void changeNumCustomers()
    {
        //int numC = numCustomers;

        //if (numC > 6)
        //    AudioManager.playTheme(2, false);
        //else if (numC > 3)
        //    AudioManager.playTheme(1, false);
        //else
        //    AudioManager.playTheme(0, false);
    }
}
