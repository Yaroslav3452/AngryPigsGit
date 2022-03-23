using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGridManager : MonoBehaviour
{
    public static LevelGridManager Current;
    //
    [SerializeField] private GameObject line1;
    [SerializeField] private GameObject line2;
    [SerializeField] private GameObject line3;
    [SerializeField] private GameObject line4;
    [SerializeField] private GameObject line5;
    [SerializeField] private GameObject line6;
    [SerializeField] private GameObject line7;
    [SerializeField] private GameObject line8;
    [SerializeField] private GameObject line9;
    private void Start()
    {
    }

    private void Awake()
    {
        Current = this;
    }
}
