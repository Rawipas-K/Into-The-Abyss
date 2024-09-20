using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager instance;

    public PlayerController activePlayer;
    public CharacterSelector activeCharacterSelect;

    private void Awake()
    {
        instance = this;
    }
}