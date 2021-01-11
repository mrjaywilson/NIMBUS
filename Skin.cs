using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// Author:         Jay Wilson
/// Description:    Handles different skins for the player.
/// 
/// Currently Deprecated.
/// 
/// </summary>
public class Skin : MonoBehaviour
{

    private List<Skin> _skinList;

    public string Name { get; set; }
    public string Address { get; set; }
    public bool Purchaseable { get; set; }
    public int GemRequirement { get; set; }

    public Skin()
    {
    }

    /// <summary>
    /// Method called after instantiation and before the furst update loop frame
    /// </summary>
    void Start()
    {

        //_skinList = new List<Skin>()
        //{
        //    new Skin
        //    {
        //        Name = "Bob",
        //        Address = "Assets/Sprites/Player/bob.png",
        //        Purchaseable = false,
        //        GemRequirement = 0
        //    }
        //};

    }

    /// <summary>
    /// Method to create the skin list.
    /// </summary>
    public void CreateList()
    {
        _skinList = new List<Skin>()
        {
            new Skin
            {
                Name = "Bob",
                Address = "Assets/Sprites/Player/bob.png",
                Purchaseable = false,
                GemRequirement = 0
            }
        };
    }

    /// <summary>
    /// Method to get the current skin list.
    /// </summary>
    /// <returns></returns>
    public List<Skin> GetList()
    {
        return _skinList;
    }

}
