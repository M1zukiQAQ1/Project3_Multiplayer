using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IDamagable
{
    public float TotalHealth { get; set; }
    public NetworkVariable<float> CurrentHealth { get; set; }

    public void Death();

}
