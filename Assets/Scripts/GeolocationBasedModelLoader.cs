using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARBuildingLoader : MonoBehaviour
{
    [Header("Настройки геолокации")]
    [SerializeField] private float updateInterval = 5f; // Интервал обновления координат

    [Header("Настройки корпусов")]
    [SerializeField] private BuildingData[] buildings; // Данные о корпусах

    [Header("AR Components")]
    [SerializeField] private GameObject[] floorModels; // Модели этажей
    [SerializeField] private GameObject arContents; // Родительский объект для AR-контента

    [Header("UI Components")]
    [SerializeField] private GameObject floorSelectionPanel;
    [SerializeField] private TMP_Text statusText;

    private string currentBuildingId;
    private bool isTracking;

    // Данные для корпуса
    [System.Serializable]
    public class BuildingData
    {
        public string buildingId;
        public Vector2 centerCoordinates;
        public float radius = 0.0005f; // ~50 метров
    }

    void Start()
    {
        InitializeLocationService();
        DisableAllModels();
        floorSelectionPanel.SetActive(false);
    }

    void InitializeLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            statusText.text = "Геолокация отключена!";
            return;
        }

        Input.location.Start(1f, 1f);
        InvokeRepeating(nameof(UpdateLocation), 0, updateInterval);
    }

    void UpdateLocation()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            statusText.text = "Поиск GPS...";
            return;
        }

        CheckProximityToBuildings(new Vector2(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        ));
    }

    void CheckProximityToBuildings(Vector2 currentCoords)
    {
        foreach (var building in buildings)
        {
            float distance = Vector2.Distance(currentCoords, building.centerCoordinates);

            if (distance <= building.radius)
            {
                if (currentBuildingId != building.buildingId)
                {
                    currentBuildingId = building.buildingId;
                    OnEnterBuildingZone(building.buildingId);
                }
                return;
            }
        }

        if (!string.IsNullOrEmpty(currentBuildingId))
        {
            OnExitBuildingZone();
        }
    }

    void OnEnterBuildingZone(string buildingId)
    {
        statusText.text = $"Корпус: {buildingId}";
        floorSelectionPanel.SetActive(true);
        isTracking = true;
    }

    void OnExitBuildingZone()
    {
        currentBuildingId = null;
        floorSelectionPanel.SetActive(false);
        DisableAllModels();
        statusText.text = "Вне зоны университета";
        isTracking = false;
    }

    public void SelectFloor(int floorIndex)
    {
        if (!isTracking) return;

        DisableAllModels();
        floorModels[floorIndex].SetActive(true);
        statusText.text = $"{currentBuildingId} • Этаж {floorIndex + 1}";
    }

    void DisableAllModels()
    {
        foreach (var model in floorModels)
        {
            model.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (Input.location.isEnabledByUser)
        {
            Input.location.Stop();
        }
    }
}
