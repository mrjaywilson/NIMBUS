using UnityEngine;

[CreateAssetMenu(menuName = "New Gem")]
public class Collectible : ScriptableObject
{
    [SerializeField]
    private GameObject _gem;
    [SerializeField]
    private float _temperature;
    [SerializeField]
    private float _tint;
    [SerializeField]
    private int _scoreValue;

    public GameObject Gem { get { return _gem; } set { _gem = value; } }
    public float Temperature { get { return _temperature; } set { _temperature = value; } }
    public float Tint { get { return _tint; } set { _tint = value; } }
    public int ScoreValue { get { return _scoreValue; } set { _scoreValue = value; } }
}
