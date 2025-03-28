using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DominoGames.Core.EventSystem
{
    public enum EEventTypes
    {
        None,



        // joystick inputs
        Joystick_OnBegin,
        Joystick_OnUpdate,
        Joystick_OnEnd,



        // EnemySpawner Events,
        EnemySpawner_OnConfigUpdate,



        // character events
        Character_Init,
        Character_Destroy,


        // enemy events
        Enemy_Init,
        Enemy_Damaged,
        Enemy_Dead,


        // boss events
        Boss_Init,
        Boss_Damaged,
        Boss_Dead,

        Boss_BreakPointChanged,


        // player events
        // move events
        Player_Move_Begin,
        Player_Move_Update,
        Player_Move_End,

        Player_Damaged,
        Player_Dead,

        Player_Attack,
        Player_Skill_Ultimate,
        Player_Skill_Normal,
    }
}
