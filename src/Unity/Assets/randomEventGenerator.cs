using UnityEngine;

public class randomEventGenerator : MonoBehaviour
{

    public GameObject tornado = null;
    public GameObject monster = null;
    public GameObject storm = null;

    public AudioSource tornadoSound = null;
    public AudioSource monsterSound = null;
    public AudioSource stormSound = null;

    public GameObject arEventsContainer = null;

    int eventRarity = 400;

    private float timer;
    private float activeEvent = -1;
    public float eventDuration;

    private void Start()
    {
        timer = Time.time;
    }

    void Update()
    {
        if (!arEventsContainer.active)
        {
            int randomNumber = Random.Range(0, eventRarity);
            if (randomNumber == 50)
            {
                arEventsContainer.SetActive(true);
                activeEvent = Random.Range(0, 3);
                switch (activeEvent)
                {
                    case 0:
                        tornadoSound.Play();
                        tornado.SetActive(true); break;
                    case 1:
                        monsterSound.Play();
                        monster.SetActive(true); break;
                    case 2:
                        stormSound.Play();
                        storm.SetActive(true); break;
                }
                timer = Time.time;
            }
        }
        else
        {
            if(Time.time - timer >= eventDuration)
            {

                tornado.SetActive(false); 
                monster.SetActive(false); 
                storm.SetActive(false);
                arEventsContainer.SetActive(false);

                activeEvent = -1;

            }
        }
        
    }
}
