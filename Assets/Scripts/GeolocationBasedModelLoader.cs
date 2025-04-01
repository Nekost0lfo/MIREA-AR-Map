using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ARBuildingLoader : MonoBehaviour
{
    [Header("��������� ����������")]
    [SerializeField] private float updateInterval = 5f; // �������� ���������� ���������

    [Header("��������� ��������")]
    [SerializeField] private BuildingData[] buildings; // ������ � ��������

    [Header("AR Components")]
    [SerializeField] private GameObject[] floorModels; // ������ ������
    [SerializeField] private GameObject arContents; // ������������ ������ ��� AR-��������

    [Header("UI Components")]
    [SerializeField] private GameObject floorSelectionPanel;
    [SerializeField] private TMP_Text statusText;

    private string currentBuildingId;
    private bool isTracking;

    // ������ ��� �������
    [System.Serializable]
    public class BuildingData
    {
        public string buildingId;
        public Vector2 centerCoordinates;
        public float radius = 0.0005f; // ~50 ������
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
            statusText.text = "���������� ���������!";
            return;
        }

        Input.location.Start(1f, 1f);
        InvokeRepeating(nameof(UpdateLocation), 0, updateInterval);
    }

    void UpdateLocation()
    {
        if (Input.location.status != LocationServiceStatus.Running)
        {
            statusText.text = "����� GPS...";
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
        statusText.text = $"������: {buildingId}";
        floorSelectionPanel.SetActive(true);
        isTracking = true;
    }

    void OnExitBuildingZone()
    {
        currentBuildingId = null;
        floorSelectionPanel.SetActive(false);
        DisableAllModels();
        statusText.text = "��� ���� ������������";
        isTracking = false;
    }

    public void SelectFloor(int floorIndex)
    {
        if (!isTracking) return;

        DisableAllModels();
        floorModels[floorIndex].SetActive(true);
        statusText.text = $"{currentBuildingId} � ���� {floorIndex + 1}";
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
