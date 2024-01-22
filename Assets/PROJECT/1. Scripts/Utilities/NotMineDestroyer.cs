using Photon.Pun;

public class NotMineDestroyer : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }
}
