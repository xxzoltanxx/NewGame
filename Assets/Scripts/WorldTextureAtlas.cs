using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTextureAtlas : MonoBehaviour
{
    public Texture2D GrassBasic;
    public Texture2D WaterLU;
    public Texture2D WaterU;
    public Texture2D WaterRU;
    public Texture2D WaterR;
    public Texture2D WaterRD;
    public Texture2D WaterD;
    public Texture2D WaterDL;
    public Texture2D WaterL;
    public Texture2D WaterNone;
    public Texture2D WaterAll;
    public Texture2D WaterLUD;
    public Texture2D WaterDU;
    public Texture2D WaterRUD;
    public Texture2D WaterLRU;
    public Texture2D WaterLR;
    public Texture2D WaterLRD;
    public Texture2D Mountain;
    public Texture2D Tree;
    public Texture2D Village;
    public Texture2D Road;
    public enum Tiles
    {
        GrassBasic = 0,
        WaterLU = 1,
        WaterU = 2,
        WaterRU = 3,
        WaterR = 4,
        WaterRD = 5,
        WaterD = 6,
        WaterDL = 7,
        WaterL = 8,
        WaterNone = 9,
        WaterAll = 10,
        WaterLUD = 11,
        WaterDU = 12,
        WaterRUD = 13,
        WaterLRU = 14,
        WaterLR = 15,
        WaterLRD = 16,
        Mountain = 17,
        Tree = 18,
        Village = 19,
        Road = 20,
        Count = 21
    }
    public Dictionary<Tiles, Rect> tileMapRects = new Dictionary<Tiles, Rect>();
    public Texture2D packedTexture;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Construct()
    {
        Texture2D[] textures = new Texture2D[(int)Tiles.Count];
        textures[0] = GrassBasic;
        textures[1] = WaterLU;
        textures[2] = WaterU;
        textures[3] = WaterRU;
        textures[4] = WaterR;
        textures[5] = WaterRD;
        textures[6] = WaterD;
        textures[7] = WaterDL;
        textures[8] = WaterL;
        textures[9] = WaterNone;
        textures[10] = WaterAll;
        textures[11] = WaterLUD;
        textures[12] = WaterDU;
        textures[13] = WaterRUD;
        textures[14] = WaterLRU;
        textures[15] = WaterLR;
        textures[16] = WaterLRD;
        textures[17] = Mountain;
        textures[18] = Tree;
        textures[19] = Village;
        textures[20] = Road;
        

        packedTexture = new Texture2D(1100, 1100);
        Rect[] rects;
        rects = packedTexture.PackTextures(textures, 2, 1500);

        for (int i = 0; i < (int)Tiles.Count; ++i)
        {
            tileMapRects[(Tiles)i] = rects[i];
        }
    }
}
