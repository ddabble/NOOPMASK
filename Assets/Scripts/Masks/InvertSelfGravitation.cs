using UnityEngine;

public class InvertSelfGravitation : MonoBehaviour, Mask
{
    public string DisplayedMaskName => "Newton no more ニュートン";
    [SerializeField]
    private GameObject prefabLevel;
    [SerializeField]
    private GameObject realLevel;
    private GameObject tempLevel;
    private float originalHeight;
    [SerializeField]
    private Transform player;
    [SerializeField]
    private CharacterController playerController;
    public void OnEquipped()
    {
        realLevel.SetActive(false);
        tempLevel = Instantiate(prefabLevel);
        originalHeight = player.transform.position.y;
    }

    public void OnUnequipped()
    {
        realLevel.SetActive(true);
        Destroy(tempLevel);
        realLevel.transform.position = new Vector3(realLevel.transform.position.x, -(originalHeight - player.transform.position.y) * 2f, realLevel.transform.position.z);
        //player.transform.position = new Vector3(0, player.transform.position.y + (originalHeight - player.transform.position.y)*2f, 0);
        //playerController.Move(new Vector3(0, player.transform.position.y + (originalHeight - player.transform.position.y) * 2f, 0));// = new Vector3(0, player.transform.position.y + (originalHeight - player.transform.position.y) * 4f, 0);
    }
}
