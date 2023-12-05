using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AnnouncementDisplay : MonoBehaviour
{
    public TextMeshProUGUI announcementText;
    public float minScale = 1.0f;
    public float maxScale = 2.0f;
    public float duration = 3.0f;

    private void Start()
    {
        // Subscribe in Start if GameManager instance is not available in OnEnable
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Announcement += DisplayAnnouncement;
        }
    }

    private void OnEnable()
    {
        // Safeguard to ensure GameManager instance is available
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Announcement += DisplayAnnouncement;
        }
        announcementText.gameObject.SetActive(false);
        
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Announcement -= DisplayAnnouncement;
        }

    }

    private void DisplayAnnouncement()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
        {
            return; // Exit if the script or GameObject is destroyed or inactive
        }

                MyDebug.Instance.Log(this.ToString(), MyDebug.Instance.B);
        StartCoroutine(AnnounceRoutine(GameManager.Instance.GetAnnouncementMessage()));
    }

    private IEnumerator AnnounceRoutine(string message)
    {
        announcementText.gameObject.SetActive(true);
        announcementText.text = message;

        float timer = 0;
        while (timer < duration)
        {
            float scale = Mathf.Lerp(minScale, maxScale, timer / duration);
            announcementText.transform.localScale = new Vector3(scale, scale, scale);

            announcementText.color = new Color(announcementText.color.r, announcementText.color.g, announcementText.color.b, (1 - (timer / duration))*2);

            timer += Time.deltaTime;
            yield return null;
        }

        announcementText.gameObject.SetActive(false);
    }
}
