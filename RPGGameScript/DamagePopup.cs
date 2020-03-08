using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform pfDamagePopup;
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;
    public DamagePopup Create(Vector3 position, int damage)
    {
        Transform damagePopUpTransform = Instantiate(pfDamagePopup, position, Quaternion.identity);
        DamagePopup damagePopup = damagePopUpTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage);
        return damagePopup; 
    }
    void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveY = 10f;
        transform.position += new Vector3(0, moveY) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Setup(int amount)
    {
        textMesh.SetText(amount.ToString());
        textColor = textMesh.color;
        disappearTimer = 1f;

    }
}
