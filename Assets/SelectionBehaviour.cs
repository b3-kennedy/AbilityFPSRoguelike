using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBehaviour : MonoBehaviour
{
    Button button;
    public string characterName;
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnSelect);
    }

    public void OnSelect()
    {
        PickScreenManager.Instance.SelectCharacterServerRpc(NetworkManager.Singleton.LocalClientId, transform.GetSiblingIndex());
    }
}
