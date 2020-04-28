/*
    Copyright 2020, Pablo Ignacio Marquez Tello aka Morgolock, All rights reserved.
    Argentum Online Clasico
    noland.studios@gmail.com
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void PlayGame(){
      SceneManager.LoadScene("World");
   }

   public void QuitGame(){
      Debug.Log("QuitGame");
      Application.Quit();
   }
}
