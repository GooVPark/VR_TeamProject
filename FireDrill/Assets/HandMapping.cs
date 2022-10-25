using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMapping : MonoBehaviour
{
    public Transform targetThumb1;
    public Transform targetThumb2;
    public Transform targetThumb3;

    public Transform targetIndex1;
    public Transform targetIndex2;
    public Transform targetIndex3;

    public Transform targetMiddle1;
    public Transform targetMiddle2;
    public Transform targetMiddle3;

    public Transform targetRing1;
    public Transform targetRing2;
    public Transform targetRing3;

    public Transform targetPinky1;
    public Transform targetPinky2;
    public Transform targetPinky3;

    public Transform myThumb1;
    public Transform myThumb2;
    public Transform myThumb3;

    public Transform myIndex1;
    public Transform myIndex2;
    public Transform myIndex3;

    public Transform myMiddle1;
    public Transform myMiddle2;
    public Transform myMiddle3;

    public Transform myRing1;
    public Transform myRing2;
    public Transform myRing3;

    public Transform myPinky1;
    public Transform myPinky2;
    public Transform myPinky3;

    public void Mapping(Transform target, Transform my)
    {
        Quaternion targetRotation = target.localRotation;
        Quaternion corvertedRotation = targetRotation * Quaternion.AngleAxis(90, Vector3.forward);

        my.localRotation = corvertedRotation;
    }

    private void Update()
    {
        Mapping(targetThumb1, myThumb1);
        Mapping(targetThumb2, myThumb2);
        Mapping(targetThumb2, myThumb2);

        Mapping(targetIndex1, myIndex1);
        Mapping(targetIndex2, myIndex2);
        Mapping(targetIndex3, myIndex3);

        Mapping(targetMiddle1, myMiddle1);
        Mapping(targetMiddle2, myMiddle2);
        Mapping(targetMiddle3, myMiddle3);

        Mapping(targetRing1, myRing1);
        Mapping(targetRing2, myRing2);
        Mapping(targetRing3, myRing3);

        Mapping(targetPinky1, myPinky1);
        Mapping(targetPinky2, myPinky2);
        Mapping(targetPinky3, myPinky3);
    }
}
