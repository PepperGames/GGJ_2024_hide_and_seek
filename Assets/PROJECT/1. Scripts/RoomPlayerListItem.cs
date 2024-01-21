using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomPlayerListItem : MonoBehaviour
{
    public TMP_Text nicknameTXT;
    public TMP_Text teamTXT;

    public void Initialize(string _nickname, Team _teamName)
    {
        nicknameTXT.text = _nickname;
        teamTXT.text = _teamName.ToString();
    }
}
