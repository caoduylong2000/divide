using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ES3Types;


public class MoneySystem : MonoBehaviour
{
    public static MoneySystem Instance; // Current player money

    // Reference to the UI Text component to display the balance
    public int gp;
    public Text moneyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Load currency from save
        gp = ES3.Load<int>("gp", 0);
        UpdateUI();
    }

    // Add money to the player's balance
    public void AddMoney(int amount)
    {
        gp += amount;
        //Debug.Log("Added " + amount + " money. Current balance: " + PlayerMoney);
        UpdateUI();
        SaveCurrency();
        //return 0;
    }

    //public int AddMoney(GameModeInfo gameMode, string ranking, int amount)
    //{
    //    //PlayerMoney += amount;
    //    //Debug.Log("Added " + amount + " money. Current balance: " + PlayerMoney);
    //    //UpdateUI();
    //    return 0;
    //}

    // Subtract money from the player's balance
    public int SubtractMoney(int amount)
    {

        if (gp >= amount)
        {
            gp -= amount;
            Debug.Log("Subtracted " + amount + " money. Current balance: " + gp);
            UpdateUI();
            SaveCurrency();
            return 1;
        }
        else
        {
            Debug.Log("Insufficient funds!");
            UpdateUI();
            return -1;
        }
    }

    // Update the UI to display the current balance
    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = gp.ToString();
        }
    }

    private void SaveCurrency()
    {
        ES3.Save<int>("gp", gp);
    }

    private void LoadCurrency()
    {
        gp = ES3.Load<int>("gp", gp);
    }
}
