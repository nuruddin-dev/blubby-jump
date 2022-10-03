using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject optionButton, homePanel, selectSkyboxPanel, game, mainCamera;
    public GameObject soundButton, musicButton;
    public GameObject gameInstruction, selectSkyboxInstruction;
    public Text scoreText, highScoreText;
    public Camera secCam;
    public Material[] skyboxs; // Assign all skybox **NOT MATERIAL** via inspector
    public GameObject[] previewSkyboxs; // Assign all sphere (preview skyboxs) in correct order via inspector

    static bool isReplay = false;

    // public because these need to be saved
    public bool canMusic = true, canSound = true;
    public int highScore = 0;
    public int skyboxPos = 2;
    public float[] previewSkyboxsPos;

    // Positions for moving grounds on random position
    public float[] zPositions;

    // AudioSources
    AudioSource homeAudioSource;
    AudioSource musicAudioSource;
    AudioSource soundAudioSource;

    // AudioClips
    public AudioClip homeAudioClip;
    public AudioClip musicAudioClip; // For Background music
    public AudioClip squeeze, jump, land; // For blubby sounds

    // Awake is called first when the GAME_OBJECT is created
    void Awake()
    {
        previewSkyboxsPos = new float[15];

        if (PlayerPrefs.HasKey("GameOpenedForFirstTime"))
        {
            LoadData();
            SetSkyboxInPosition();
            // gameInstruction.SetActive(false);
            // selectSkyboxInstruction.SetActive(false);
        }
        else
        {
            SaveData();
            PlayerPrefs.SetInt("GameOpenedForFirstTime", 0);
            gameInstruction.SetActive(true);
            selectSkyboxInstruction.SetActive(true);
        }
    }
    
    void SetSkyboxInPosition()
    {
        for(int i=0; i<5; i++)
            {
                float x = previewSkyboxsPos[i];
                float y = previewSkyboxsPos[i+5];
                float z = previewSkyboxsPos[i+10];
                previewSkyboxs[i].transform.position = new Vector3(x, y, z);
            }
    }

    // Start is called before the first frame update
    void Start()
    {
        optionButton.SetActive(false);
        RenderSettings.skybox = skyboxs[skyboxPos];


        // Initiating AUDIOSOURCES
        homeAudioSource = AddAudio (false, false, 1f);
        soundAudioSource = AddAudio (false, false, 1f);
        musicAudioSource = AddAudio (true, false, 1f); // Arguments: (bool loop, bool playAwake, float volume)

        // This method will decide what to response on various response button clicked
        ResponseOfOptionButtons();

        SetAudioButtonsColor();
        
        // Create random positions for ground placement
        CreateRandomPosition();
    }

    void ResponseOfOptionButtons()
    {
        if(!isReplay){
            homePanel.SetActive(true);
            selectSkyboxPanel.SetActive(false);
            game.SetActive(false);
            if(canMusic)
            {
                homeAudioSource.clip = homeAudioClip;
                homeAudioSource.Play();
            } 
        }else{
            ClickedGoButton();
            if(canMusic)
            {
                musicAudioSource.clip = musicAudioClip;
                musicAudioSource.Play();
            } 
        }
    }

    AudioSource AddAudio(bool loop, bool playAwake, float volume) 
    { 
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = loop;
        audioSource.playOnAwake = playAwake;
        audioSource.volume = volume; 
        return audioSource; 
    }

    void SetAudioButtonsColor()
    {
        if(canSound)
            soundButton.GetComponentInChildren<Text>().color = new Color32(255,255,255,100);
        else
            soundButton.GetComponentInChildren<Text>().color = new Color32(255,0,0,100);
        if(canMusic)
            musicButton.GetComponentInChildren<Text>().color = new Color32(255,255,255,100);
        else
            musicButton.GetComponentInChildren<Text>().color = new Color32(255,0,0,100);
    }

    void CreateRandomPosition()
    {
        zPositions = new float[100];
        float lowBound = 200;
        for(int i=0; i<100; i++){
            float rand = Random.Range(lowBound+15, lowBound+30);
            zPositions[i] = rand;
            lowBound = rand;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1)
        {
            scoreText.text = "Score: " + Blubby.score;
            if(Blubby.score > highScore){
                highScore = Blubby.score;
            }
        }
        else
        {
            highScoreText.text ="High Score: " + highScore;
            optionButton.SetActive(true);
            musicAudioSource.Stop();
        }

        if(selectSkyboxPanel.activeSelf)
            SelectSkybox();
    }

    void SelectSkybox()
    {
        // This condition checks if player is in selecting skybox mat 
        if (!isReplay && Input.GetMouseButtonDown(0))
        {
            Ray ray = secCam.ScreenPointToRay(Input.mousePosition);
            // Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                Vector3 hitPos = hit.collider.transform.position;
                hit.collider.transform.position = new Vector3(0f, 0f, 3.5f);

                for(int i = 0; i<5; i++){
                    if(previewSkyboxs[i].name != hit.collider.name && previewSkyboxs[i].transform.position.x == 0)
                        previewSkyboxs[i].transform.position = hitPos;
                    if(previewSkyboxs[i].name == hit.collider.name)
                    {
                        RenderSettings.skybox = skyboxs[i];
                        skyboxPos = i;
                    }
                }
                
                for(int i=0; i<5; i++){
                    previewSkyboxsPos[i] = previewSkyboxs[i].transform.position.x;
                    previewSkyboxsPos[i+5] = previewSkyboxs[i].transform.position.y;
                    previewSkyboxsPos[i+10] = previewSkyboxs[i].transform.position.z;
                }

                SaveData();
            }
        }
    }

    // This method will call when play button clicked from HOME_PANEL
    public void ClickedPlayButton(){
        homePanel.SetActive(false);
        selectSkyboxPanel.SetActive(true);
        game.SetActive(false);
        mainCamera.SetActive(false);
        if(canMusic)
        {
            musicAudioSource.clip = musicAudioClip;
            musicAudioSource.Play();
        } 
    }

    // This method will call when play button clicked from SELECT_SKYBOX_PANEL
    public void ClickedGoButton(){
        mainCamera.SetActive(true);
        optionButton.SetActive(false);
        homePanel.SetActive(false);
        selectSkyboxPanel.SetActive(false);
        game.SetActive(true);
        RenderSettings.skybox = skyboxs[skyboxPos];
    }

    // Make response to click of AUDIO BUTTONS (music or sound)
    public void ClickedAudioButton(string audioName){
        if(audioName == "music")
        {
            canMusic = !canMusic;
            if(canMusic)
            {
                musicButton.GetComponentInChildren<Text>().color = new Color32(255,255,255,100);
                musicAudioSource.clip = musicAudioClip;
                musicAudioSource.Play();
            }
            else
            {
                musicButton.GetComponentInChildren<Text>().color = new Color32(255,0,0,100);
                musicAudioSource.Stop();
            }
        }
        if(audioName == "sound")
        {
            canSound = !canSound;

            if(canSound)
                soundButton.GetComponentInChildren<Text>().color = new Color32(255,255,255,100);
            else
                soundButton.GetComponentInChildren<Text>().color = new Color32(255,0,0,100);
        }
        
        SaveData();
    }

    // This function will call from Replay and Home button [for replay bool home = 0]
    public void ClickedReplayButton(bool home)
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //Time.timeScale = 1f;
        if(home)
            isReplay = false;
        else
            isReplay = true;
            
        Ground.lastArrayPos = 0;
        SaveData();
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene",LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
            Time.timeScale = 1f;
        }
    }

    // This method will call when blubby in action from Blubby class
    public void PlaySound(string soundAudioClip)
    {
        switch (soundAudioClip)
        {
            case "squeeze": 
                soundAudioSource.clip = squeeze;
                break;
            case "jump" :
                soundAudioSource.clip = jump;
                break;
            case "land" :
                soundAudioSource.clip = land;
                break;
            default:
                break;
        }
        soundAudioSource.Play();
    }

    // Saving DATA to data.graphicless file on devices
    public void SaveData()
    {
        SaveSystem.SaveData(this);
    }

    // Loading DATA from data.graphicless file on devices
    public void LoadData()
    {
        GameData data = SaveSystem.LoadData();
        this.highScore = data.highScore;
        this.skyboxPos = data.skyboxPos;
        this.previewSkyboxsPos = data.previewSkyboxsPos;
        this.canMusic = data.canMusic;
        this.canSound = data.canSound;
    }

    public void Exit(){
        Application.Quit();
    }
}
