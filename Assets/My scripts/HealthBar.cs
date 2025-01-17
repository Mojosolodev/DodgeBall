using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image HealthBarImage;
    private Camera cam;
    
    private void Start()
    {
        cam=Camera.main;
    }
    private void update()
    {
        transform.rotation=Quaternion.LookRotation(transform.position-cam.transform.position);
    }

    public void UpdateHealthBar(float maxHealth,float CurrentHealth)
    {
        HealthBarImage.fillAmount=CurrentHealth/maxHealth;
    }
}
