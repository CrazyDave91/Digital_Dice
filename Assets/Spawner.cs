using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] Dropdown countDropdown;
    [SerializeField] Transform cam;
    [SerializeField] GameObject ContrastBackground;
    [SerializeField] AudioSource audioSource;

    float distanceToCamera = 6f;
    float distanceBetweenDice = 2f;
    float dicePerRow = 3f;
    float timeDelay = 1f;

    public void SpawnDice(GameObject dice)
    {
        audioSource.Play();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for(int i  = 0; i < countDropdown.value + 1; i++)
        {
            GameObject obj = Instantiate(dice);
            obj.transform.position = transform.position;
            obj.transform.parent = transform;
            // distance to camera
            float offset = 1f;
            if (dice.transform.name == "D4") offset = 0.93f;
            if (dice.transform.name == "D6") offset = 0.97f;
            if (dice.transform.name == "D8") offset = 0.96f;
            if (dice.transform.name == "D10") offset = 0.98f;
            Vector3 showPos = cam.position + cam.forward * distanceToCamera * offset;

            // hotizontal Grid
            float horizontalIndex = ((float)i % dicePerRow);
            float horizontalMaxIndex = Mathf.Clamp(countDropdown.value, 0f, dicePerRow - 1f);
            showPos += cam.right * distanceBetweenDice * horizontalIndex;
            showPos -= cam.right * distanceBetweenDice * horizontalMaxIndex / 2f;
            // vertical Grid
            float verticalIndex = Mathf.Floor((float)i / dicePerRow);
            float verticalMaxIndex = Mathf.Floor((float)countDropdown.value / dicePerRow);

            showPos += cam.up * distanceBetweenDice * verticalIndex;
            showPos -= cam.up * distanceBetweenDice * verticalMaxIndex / 2f;
            obj.GetComponent<DiceStarter>().showPosition = showPos;
        }
    }

    private void Update()
    {
        int numberOfCompleted = 0;
        int numberOfChilds = 0;
        foreach (Transform child in transform)
        {
            if (child.name == "Origin") numberOfCompleted++;
            numberOfChilds++;
        }
        if(numberOfChilds == 0)
        {
            ContrastBackground.SetActive(false);
            timeDelay = 0.5f;
        }
        else
        {
            if (numberOfCompleted != numberOfChilds)
            {
                ContrastBackground.SetActive(false);
                timeDelay = 0.5f;
            }
            else
            {
                if(timeDelay < 0f) ContrastBackground.SetActive(true);
                timeDelay -= Time.deltaTime;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
