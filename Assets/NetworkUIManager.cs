using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkUIManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public GameObject startCam;
    GameObject canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hostButton.onClick.AddListener(Host);
        joinButton.onClick.AddListener(Join);
        canvas = transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Host()
    {
        NetworkManager.Singleton.StartHost();
        DisableObjects();
        NetworkManager.Singleton.SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
    }

    void Join()
    {
        NetworkManager.Singleton.StartClient();
        DisableObjects();
    }

    void DisableObjects()
    {
        //startCam.SetActive(false);
        canvas.SetActive(false);
    }
}
