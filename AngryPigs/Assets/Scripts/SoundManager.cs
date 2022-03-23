using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   public static SoundManager Current;
   [SerializeField] private AudioSource audioSource;
   private void Awake()
   {
      Current = this;
   }
}
