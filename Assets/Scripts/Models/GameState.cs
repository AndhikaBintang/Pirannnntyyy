[System.Serializable]
public class GameState
{
    public int id;
    public string playerName;
    public int health;
    public float positionX;
    public float positionY;
    public float positionZ;
    public string currentLevel;
    public string inventoryJson;
    public System.DateTime lastSavedAt;
}