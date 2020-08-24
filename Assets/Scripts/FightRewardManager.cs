﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FightRewardManager : MonoBehaviour
{
    // OO objects
    private Queue<Action> uiQueue;
    private static readonly RandomGenerator randGen = new RandomGenerator();

    // Unity objects
    public TextMeshProUGUI dropStat;
    public Sprite weaponSprite;
    public Sprite helmetSprite;
    public Sprite shieldSprite;
    public Image lastDropImage;

    public void Start()
    {
        uiQueue = new Queue<Action>();

        Item drop;
        (int level, bool success1) = DataManager.LoadLevelIndex();
        if (!success1)
        {
            Debug.LogWarning("Could not load level reward range!");
            drop = null;
            uiQueue.Enqueue(() => lastDropImage.sprite = null);
            uiQueue.Enqueue(() => lastDropImage.enabled = false);
            uiQueue.Enqueue(() => dropStat.SetText(""));
        } 
        else
        {
            drop = randGen.RandomItem(level);
            bool success2 = DataManager.SaveLevelRewardItem(drop);
            if (!success2)
            {
                Debug.LogError("Could not save level reward item!"); // if we hit here, it'll be noticeable since we disabled UI elems
                // drop = null; ???
                uiQueue.Enqueue(() => lastDropImage.sprite = null);
                uiQueue.Enqueue(() => lastDropImage.enabled = false);
                uiQueue.Enqueue(() => dropStat.SetText(""));
            }
            else
            {
                Debug.Log("Saved level reward item!");

                switch (drop.ItemType)
                {
                    case ItemType.WEAPON:
                        uiQueue.Enqueue(() => lastDropImage.sprite = weaponSprite);
                        break;
                    case ItemType.HELMET:
                        uiQueue.Enqueue(() => lastDropImage.sprite = helmetSprite);
                        break;
                    case ItemType.SHIELD:
                        uiQueue.Enqueue(() => lastDropImage.sprite = shieldSprite);
                        break;
                    default:
                        uiQueue.Enqueue(() => lastDropImage.sprite = null);
                        break;
                }
                uiQueue.Enqueue(() => lastDropImage.enabled = true);
                uiQueue.Enqueue(() => dropStat.SetText(string.Format("+{0}", drop.StatIncrease)));
            }            
        }
    }

    public void Update()
    {
        int amount = uiQueue.Count;
        for (int i = 0; i < amount; ++i)
        {
            uiQueue.Dequeue().Invoke();
        }
    }

    public void LoadMap()
    {
        SceneManager.LoadScene(SceneIndex.MAP_INDEX);
    }
}