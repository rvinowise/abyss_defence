using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using rvinowise.unity.units.parts.limbs;
using rvinowise.unity.units.parts;
using rvinowise.unity.units.control;

namespace rvinowise.unity.units {

[Serializable]
public class Team: MonoBehaviour
{
    public List<Team> enemies = new List<Team>();
    public List<Team> allies = new List<Team>();

    public List<Intelligence> units = new List<Intelligence>();
  

}


}