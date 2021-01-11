using UnityEngine;

/// <summary>
/// Author:         Jay Wilson
/// Description:    Scriptable object for streamlining the creation
///                 of new gems.
/// </summary>
[CreateAssetMenu(menuName = "New Gem")]
public class Collectible : ScriptableObject
{
    // Properties
    [SerializeField]
    private GameObject _gem;
    [SerializeField]
    private float _temperature;
    [SerializeField]
    private float _tint;
    [SerializeField]
    private int _scoreValue;

    public GameObject Gem { get { return _gem; } set { _gem = value; } }                        // Gem
    public float Temperature { get { return _temperature; } set { _temperature = value; } }     // Temperature color of the gem
    public float Tint { get { return _tint; } set { _tint = value; } }                          // Tint of the Gem
    public int ScoreValue { get { return _scoreValue; } set { _scoreValue = value; } }          // Value of the gem
}
