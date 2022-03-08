using UnityEngine;
using Mirror;

/// Customized from Mirror.NetworkManagerHUD
/// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
[DisallowMultipleComponent]
[AddComponentMenu("Network/NetHUD")]
[RequireComponent(typeof(NetworkManager))]
[HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
public class NetHUD : MonoBehaviour
{
    NetworkManager manager;

    public int offsetX = 30;
    public int offsetY = 160;
    public int areaWidth = 430;
    public int buttonHeight = 60;
    public int fontSize = 30;
    public string hostAddress = "";

    bool Button(string text)
    {
        return GUILayout.Button(text, GUILayout.Height(buttonHeight));
    }

    string TextField(string text)
    {
        return GUILayout.TextField(text, GUILayout.Height(buttonHeight));
    }

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    void OnGUI()
    {
        hostAddress = "34.134.196.226";
        GUI.skin.label.fontSize = fontSize;
        GUI.skin.button.fontSize = fontSize;
        GUI.skin.textField.fontSize = fontSize;

        GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, areaWidth, 9999));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            if (Button("Client Ready"))
            {
                NetworkClient.Ready();
                if (NetworkClient.localPlayer == null)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }

        StopButtons();

        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (Button("Host (Server + Client)"))
                {
                    manager.StartHost();
                }
            }

            // Client + IP
            GUILayout.BeginHorizontal();
            if (Button("Client"))
            {
                manager.networkAddress = hostAddress;
                manager.StartClient();
            }

            TextField(hostAddress);
            GUILayout.EndHorizontal();

            if (Button("Local Client"))
            {
                manager.networkAddress = "localhost";
                manager.StartClient();
            }
            
            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            {
                if (Button("Server Only")) manager.StartServer();
            }
        }
        else
        {
            // Connecting
            GUILayout.Label("Connecting to " + manager.networkAddress + "..");
            if (Button("Cancel Connection Attempt"))
            {
                manager.StopClient();
            }
        }
    }

    void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}");
        }
        // server only
        else if (NetworkServer.active)
        {
            GUILayout.Label($"<b>Server</b>: running via {Transport.activeTransport}");
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.activeTransport}");
        }
    }

    void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (Button("Stop Host"))
            {
                manager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (Button("Stop Client"))
            {
                manager.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (Button("Stop Server"))
            {
                manager.StopServer();
            }
        }
    }
}
