using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{

    GameObject[] blocks;    //list of placed and placable blocks
    List<GameObject> restBlocks = new List<GameObject>();   //list of the blocks that are 'cut off' and are affected by gravity
    int blockCount;
    textScript myText;
    cameraScript myCamera;
    int score = 0;
    float acceleration = 1.01f;

    bool right = false;     //on wich side the block spawns
    float blockWidth = 7f;  //width of the blocks
    bool on = true;         //true if the block is moving towards the player
    float speed = 0.1f;     //speed of the movinf blocks
    bool gameover = false;

    // Use this for initialization
    void Start()
    {
        myText = GameObject.Find("Text").GetComponent<textScript>();
        myCamera = GameObject.Find("Main Camera").GetComponent<cameraScript>();
        blocks = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            blocks[i] = transform.GetChild(i).gameObject;
        blockCount = blocks.Length;
        spawnBlock();
    }

    //get the index of the block that should now be moving
    int blockIndex()
    {
        if (blockCount < 1) blockCount = blocks.Length;
        return blockCount - 1;
    }

    //block on top of the stack
    int previousBlockIndex()
    {
        int spoofIndex = blockCount + 1;
        if (spoofIndex > blocks.Length) return 0;
        return spoofIndex - 1;
    }

    //block on the bottom of the stack
    int nextBlockIndex()
    {
        int spoofIndex = blockCount - 1;
        if (spoofIndex < 1) spoofIndex = blocks.Length;
        return spoofIndex - 1;
    }

    //bring a new block to the top of the screen
    void spawnBlock()
    {
        //move old blocks down
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].transform.position = blocks[i].transform.position + (Vector3.down);
        }

        //move gravity affected blocks down 
        for (int i = 0; i < restBlocks.Count; i++)
        {
            restBlocks[i].transform.position = restBlocks[i].transform.position + (Vector3.down);
        }

        //destory gravity affected blocks that are below the lowest block of our stack
        for (int i = 0; i < restBlocks.Count; i++)
        {
            if (restBlocks[i].transform.position.y < blocks[nextBlockIndex()].transform.position.y)
            {
                Destroy(restBlocks[i]);
                restBlocks.RemoveAt(i);
            }
        }

        //spawn the new block with the size of the highest block on the stack
        blocks[blockIndex()].transform.localScale = blocks[previousBlockIndex()].transform.localScale;

        //spawn either left or right
        if (right)
            blocks[blockIndex()].transform.position = new Vector3(blocks[previousBlockIndex()].transform.position.x, 0, blockWidth * 2);
        else
            blocks[blockIndex()].transform.position = new Vector3(-blockWidth * 2, 0, blocks[previousBlockIndex()].transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !gameover) placeBlock();
        moveBlock();
    }

    //stack the moving block on the stack
    void placeBlock()
    {
        float length = 0;
        float a1, a2, b1, b2;

        //determine the relevant locations of the edges of the moving and highest block
        if (right)
        {
            a1 = blocks[previousBlockIndex()].transform.position.z - blocks[previousBlockIndex()].transform.localScale.z / 2;
            a2 = blocks[previousBlockIndex()].transform.position.z + blocks[previousBlockIndex()].transform.localScale.z / 2;

            b1 = blocks[blockIndex()].transform.position.z - blocks[blockIndex()].transform.localScale.z / 2;
            b2 = blocks[blockIndex()].transform.position.z + blocks[blockIndex()].transform.localScale.z / 2;

        }
        else
        {
            a1 = blocks[previousBlockIndex()].transform.position.x - blocks[previousBlockIndex()].transform.localScale.x / 2;
            a2 = blocks[previousBlockIndex()].transform.position.x + blocks[previousBlockIndex()].transform.localScale.x / 2;

            b1 = blocks[blockIndex()].transform.position.x - blocks[blockIndex()].transform.localScale.x / 2;
            b2 = blocks[blockIndex()].transform.position.x + blocks[blockIndex()].transform.localScale.x / 2;
        }

        bool before = true;     //true if the block is cut off on the side of the stack opposite of the player

        if (a1 == b1)
        {
            length = a2 - a1;
        }
        else if (b2 > a1 && b2 < a2)
        {
            length = b2 - a1;
        }
        else if (b1 > a1 && b1 < a2)
        {
            length = a2 - b1;
            before = false;
        }

        //check if the player managed to not die
        if(length == 0)
        {
            StartCoroutine(gameOver());
            return;
        }

        //length of the side of the block that is cut off
        float restLength = (b2 - b1) - length;

        //move the camera, set the score and update the scorecount
        myCamera.moveCamera();
        score++;
        myText.updateText(score.ToString());

        //create block that is cut off
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.AddComponent<Rigidbody>();

        //add to list
        restBlocks.Add(cube);

        //place block and position gavity affected block
        if (right)
        {
            float pos = blocks[blockIndex()].transform.position.x;
            float size = blocks[blockIndex()].transform.localScale.x;
            float newPos = (((a1 + a2) / 2) + ((b1 + b2) / 2)) / 2;

            //place block on top of the stack
            blocks[blockIndex()].transform.localScale = new Vector3(size, 1, length);
            blocks[blockIndex()].transform.position = new Vector3(pos, 0, newPos);

            //resize newly created gravity affected block
            cube.transform.localScale = new Vector3(size, 1, restLength);

            //place newly created gravity affected block
            if (before) cube.transform.position = new Vector3(pos, 0, (newPos - length/2) - (restLength/2));
            else        cube.transform.position = new Vector3(pos, 0, (newPos + length / 2) + (restLength / 2));

        }
        else
        {
            float pos = blocks[blockIndex()].transform.position.z;
            float size = blocks[blockIndex()].transform.localScale.z;
            float newPos = (((a1 + a2) / 2) + ((b1 + b2) / 2)) / 2;

            //place block on top of the stack
            blocks[blockIndex()].transform.localScale = new Vector3(length, 1, size);
            blocks[blockIndex()].transform.position = new Vector3(newPos, 0, pos);

            //resize newly created gravity affected block
            cube.transform.localScale = new Vector3(restLength, 1, size);

            //place newly created gravity affected block
            if (before) cube.transform.position = new Vector3((newPos - length / 2) - (restLength / 2), 0, pos);
            else cube.transform.position = new Vector3((newPos + length / 2) + (restLength / 2), 0, pos);
        }

        right = !right;     //spwan block on the other side next time
        blockCount--;       //pic the next block
        spawnBlock();       //spawn new block
        speed *= acceleration;

    }

    //disable player input when the game is over, wait for 3 seconds
    IEnumerator gameOver()
    {
        gameover = true;
        myText.updateText(myText.getText() + "\nGame Over");
        yield return new WaitForSeconds(3);
        score = 0;
        SceneManager.LoadScene("gameScene");
        gameover = false;
    }

    //move the moving blocks
    void moveBlock()
    {
        //set correct direction
        if (blocks[blockIndex()].transform.position.x < -2 * blockWidth) on = !on;
        if (blocks[blockIndex()].transform.position.x > 2 * blockWidth) on = !on;
        if (blocks[blockIndex()].transform.position.z < -2 * blockWidth) on = !on;
        if (blocks[blockIndex()].transform.position.z > 2 * blockWidth) on = !on;


        //move block
        if (on && !right) blocks[blockIndex()].transform.Translate(new Vector3(-speed, 0, 0));
        if (!on && !right) blocks[blockIndex()].transform.Translate(new Vector3(speed, 0, 0));
        if (on && right) blocks[blockIndex()].transform.Translate(new Vector3(0, 0, -speed));
        if (!on && right) blocks[blockIndex()].transform.Translate(new Vector3(0, 0, speed));

    }
}
