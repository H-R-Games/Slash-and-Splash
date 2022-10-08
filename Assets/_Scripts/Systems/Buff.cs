using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public float BuffTieme(float _getTime, float _buffTime)  
    {
        float _setNewTime = _getTime * _buffTime;

        return _setNewTime;
    }

    public float BuffMoveSpeed(float _getMoveSpeed, float _buffMoveSpeed)
    {
        float _setNewMoveSpeed = _getMoveSpeed * _buffMoveSpeed;

        return _setNewMoveSpeed;
    }

    public float BuffDashRange(float _getDashRange, float _buffDashRange)
    {
        float _setNewDashRange = _getDashRange * _buffDashRange;

        return _setNewDashRange;
    }
}
