﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reindeer : MonoBehaviour {
  
    private static int _health = 5;
    private static int _speed = 5;
    private static int _damage = 5;

    private Enemy _reindeer = new Enemy(_health, _speed, _damage);
}