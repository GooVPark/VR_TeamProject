using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

using Photon.Pun;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private PhotonView photonView;

    [SerializeField] private ActionBasedController leftController;
    [SerializeField] private ActionBasedController rightController;
    private Camera headset;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        headset = Camera.main;
        leftController = GameObject.Find("LeftHand Controller").GetComponent<ActionBasedController>();
        rightController = GameObject.Find("RightHand Controller").GetComponent<ActionBasedController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            head.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            rightHand.gameObject.SetActive(false);

            MapPosition();
        }
    }

    private void MapPosition()
    {
        Vector3 headsetPosition = headset.transform.position;
        Quaternion headsetRotation = headset.transform.rotation;

        head.position = headsetPosition;
        head.rotation = headsetRotation;

        Vector3 leftPosition = leftController.transform.position;
        Quaternion leftRotation = leftController.transform.rotation;

        leftHand.position = leftPosition;
        leftHand.rotation = leftRotation;

        Vector3 rightPosition = rightController.transform.position;
        Quaternion rightRotation = rightController.transform.rotation;

        rightHand.position = rightPosition;
        rightHand.rotation = rightRotation;
    }
}
