using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bone : MonoBehaviour
{
    public int bone;
    public int numOfBones;

    public Image[] bones;
    public Sprite fullBone;
    public Sprite emptyBone;

    // Update is called once per frame
    void Update()
    {
        if (bone > numOfBones) {
            bone = numOfBones;
        }

        for (int i = 0; i < bones.Length; i++) {
            if (i < bone) {
                bones[i].sprite = fullBone;
            } else {
                bones[i].sprite = emptyBone;
            }

            if (i < numOfBones) {
                bones[i].enabled = true;
            } else {
                bones[i].enabled = false;
            }
        }
    }

    public void addBone() {
        bone++;
    }

    public int getBone() {
        return bone;
    }
}
