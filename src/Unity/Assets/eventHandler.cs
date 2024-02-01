using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class eventHandler : MonoBehaviour
{
    // Delegate and event for thunderEvent
    public delegate IEnumerator ThunderEventHandler();
    public event ThunderEventHandler ThunderEvent;

    private float timeSinceLastCheck = 0f;
    private const float checkInterval = 1f; // Check every 1 second
    public GameObject joyStick = null;
    public GameObject sandStorm = null;
    public GameObject thunderStorm = null;
    void Start()
    {

    }

    void Update()
    {
        // Update the timer
        timeSinceLastCheck += Time.deltaTime;

        // Check if it's time to perform the check
        if (timeSinceLastCheck >= checkInterval)
        {
            timeSinceLastCheck = 0f; // Reset timer
            CheckAndTriggerThunderEvent();
        }
    }

    void CheckAndTriggerThunderEvent()
    {
        // Generate a random number between 0 and 40
        int randomNumber = UnityEngine.Random.Range(0, 50);

        // Check the number and trigger the event if it's between 31 and 40
        if (randomNumber >= 31 && randomNumber <= 40)
        {
            StartCoroutine(TriggerThunderAfterSleeper());

        }
    }

    // Event handler
    IEnumerator OnThunderEvent()
    {
        int randomNumber = UnityEngine.Random.Range(0, 5);

        // Check the number and trigger the event if it's between 31 and 40
        if (randomNumber % 2 == 0)
        {
            yield return new WaitForSeconds(10);
            joyStick.SetActive(false);
            sandStorm.SetActive(true);
            yield return new WaitForSeconds(4);
            sandStorm.SetActive(false);
            joyStick.SetActive(true);
        }
        if (randomNumber % 2 != 0)
        {
            yield return new WaitForSeconds(10);
            joyStick.SetActive(false);
            thunderStorm.SetActive(true);
            yield return new WaitForSeconds(4);
            thunderStorm.SetActive(false);
            joyStick.SetActive(true);
        }
        yield return new WaitForSeconds(10);
    }
    IEnumerator Sleeper()
    {
        yield return new WaitForSeconds(15);
    }
    IEnumerator TriggerThunderAfterSleeper()
    {
        // First, call the Sleeper coroutine and wait for it to finish
        yield return StartCoroutine(Sleeper());

        // After Sleeper is done, call OnThunderEvent
        yield return StartCoroutine(OnThunderEvent());
    }
}
