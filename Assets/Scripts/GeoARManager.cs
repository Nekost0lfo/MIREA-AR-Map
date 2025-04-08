using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;

public class GeoARManager : MonoBehaviour
{
    [System.Serializable]
    public class Zone
    {
        public string zoneName;
        public Vector2 minMaxLat; 
        public Vector2 minMaxLon;
        public GameObject[] floorModels;
    }

    [Header("Настройки зон")]
    public Zone[] zones;

    [Header("AR Components")]
    public ImageTargetBehaviour imageTarget;
    public Transform modelParent; 

    [Header("UI Elements")]
    public GameObject selectionPanel;
    public Transform buttonsParent;
    public TMP_Text statusText;
    public Button backButton;
    public GameObject buttonPrefab;

    private Zone currentZone;
    private GameObject currentModel;
    private bool isTracking;

    void Start()
    {
        InitializeLocationService();
        backButton.onClick.AddListener(OnBackButton);
        selectionPanel.SetActive(false);
        backButton.gameObject.SetActive(false);

        imageTarget.OnTargetStatusChanged += OnTargetStatusChanged;
    }

    void InitializeLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            statusText.text = "Геолокация отключена!";
            return;
        }

        Input.location.Start(1f, 1f);
        InvokeRepeating(nameof(UpdateLocation), 0, 5f);
    }

    void UpdateLocation()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            statusText.text = "Поиск GPS...";
            return;
        }

        CheckCurrentZone(new Vector2(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        ));
    }

    void CheckCurrentZone(Vector2 coords)
    {
        foreach (Zone zone in zones)
        {
            if (coords.x > zone.minMaxLat.x &&
                coords.x < zone.minMaxLat.y &&
                coords.y > zone.minMaxLon.x &&
                coords.y < zone.minMaxLon.y)
            {
                if (currentZone != zone) OnEnterZone(zone);
                return;
            }
        }

        if (currentZone != null) OnExitZone();
    }

    void OnEnterZone(Zone zone)
    {
        currentZone = zone;
        statusText.text = $"Обнаружена зона: {zone.zoneName}";
        isTracking = true;

        if (currentZone.floorModels.Length == 1)
        {
            LoadFloorModel(0);
        }
    }

    void OnExitZone()
    {
        currentZone = null;
        isTracking = false;
        DestroyCurrentModel();
        selectionPanel.SetActive(false);
        backButton.gameObject.SetActive(false);
        statusText.text = "Рядом нет объектов";
    }

    void OnTargetStatusChanged(ObserverBehaviour observer, TargetStatus status)
    {
        if (status.Status == Status.TRACKED && isTracking)
        {
            if (currentZone.floorModels.Length > 1)
            {
                ShowSelectionPanel();
            }
        }
        else
        {
            selectionPanel.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
    }

    void ShowSelectionPanel()
    {
        selectionPanel.SetActive(true);
        backButton.gameObject.SetActive(true);
        CreateFloorButtons();
    }

    void CreateFloorButtons()
    {
        foreach (Transform child in buttonsParent) Destroy(child.gameObject);

        for (int i = 0; i < currentZone.floorModels.Length; i++)
        {
            int index = i;
            GameObject button = Instantiate(buttonPrefab, buttonsParent);

            button.GetComponentInChildren<TMP_Text>().text = $"Этаж {index + 1}";

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                LoadFloorModel(index);
                selectionPanel.SetActive(false);
            });
        }
    }

    void LoadFloorModel(int index)
    {
        DestroyCurrentModel();
        currentModel = Instantiate(currentZone.floorModels[index], modelParent);
        currentModel.transform.localPosition = Vector3.zero;
    }

    void OnBackButton()
    {
        if (currentZone.floorModels.Length > 1)
        {
            ShowSelectionPanel();
        }
        else
        {
            selectionPanel.SetActive(false);
            backButton.gameObject.SetActive(false);
        }
        DestroyCurrentModel();
    }

    void DestroyCurrentModel()
    {
        if (currentModel != null) Destroy(currentModel);
    }

    void OnDestroy()
    {
        if (Input.location.isEnabledByUser) Input.location.Stop();
    }
}
