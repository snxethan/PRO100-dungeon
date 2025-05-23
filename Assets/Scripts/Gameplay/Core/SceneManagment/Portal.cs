using System.Collections;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] DestinationIdentifier destinationPortal;
    [SerializeField] Transform spawnPoint;


    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        player.StartPortalTransition();
        StartCoroutine(SwitchScene());
        
    }

    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();

    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        yield return fader.FadeIn(0.5f);
        
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.SetPositionAndSnapToTile(destPortal.spawnPoint.position);

        if (sceneToLoad == 3)
            yield return new WaitForSeconds(2f);
        yield return fader.FadeOut(0.5f);
        player.EndPortalTransition();
        
        player.StartTimer(10);
        
        Destroy(gameObject);
    }

    public Transform SpawnPoint => spawnPoint;
}

public enum DestinationIdentifier
{
    A, B, C, D, E
}