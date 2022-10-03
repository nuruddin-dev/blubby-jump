using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int highScore, skyboxPos;
    public float[] previewSkyboxsPos;
    public bool canMusic, canSound;
    

    public GameData(GameManager gameManager)
    {
        this.highScore = gameManager.highScore;
        this.skyboxPos = gameManager.skyboxPos;
        this.canMusic = gameManager.canMusic;
        this.canSound = gameManager.canSound;

        previewSkyboxsPos = new float[15];
        for(int i=0; i<5; i++)
        {
            previewSkyboxsPos[i] = gameManager.previewSkyboxs[i].transform.position.x;
            previewSkyboxsPos[i+5] = gameManager.previewSkyboxs[i].transform.position.y;
            previewSkyboxsPos[i+10] = gameManager.previewSkyboxs[i].transform.position.z;
        }
    }
}
