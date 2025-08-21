
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
   private static CharacterManager _instance;
    public static CharacterManager Instance
    {
        get
        {
                if (_instance == null)
                {
                    _instance = new GameObject("CharacterManager").AddComponent<CharacterManager>();

            }
            return _instance;
        }
    }

    public Player _player;
    public Player Player
    {
        get { return _player; } 
        set { _player = value; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            // ⭐ 플레이어 게임 오브젝트를 찾아 _player에 할당합니다.
            Player = FindObjectOfType<Player>();
        }
        else
        {
            if (_instance == this)
            {
                Destroy(gameObject);
            }
        }
    }


}
