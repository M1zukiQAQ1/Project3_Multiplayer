using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumberController : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private Animator animator;

    void Start()
    {
        damageText = GetComponentInChildren<TMP_Text>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(float damage)
    {
        damageText.text = damage.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Destroy(gameObject);
        }

    }
}
