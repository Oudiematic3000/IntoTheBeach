using Unity.Netcode;
using UnityEngine;

public class NetworkAntiDuplicator : MonoBehaviour
{
    public static NetworkManager instance;
    void Start()
    {
        if (instance == null) instance = GetComponent<NetworkManager>();
        else Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
