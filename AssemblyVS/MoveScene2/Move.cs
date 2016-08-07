using UnityEngine;

public class Move
{
    private bool _moveToDown = false;
    public Vector3 GetNextPosition(Vector3 curPosition, Vector3 bounds)
    {
        if (curPosition.y >= 3 && !_moveToDown)
        {
            _moveToDown = true;
        }
        else if (curPosition.y <= 0 && _moveToDown)
        {
            _moveToDown = false;
        }
        var step = _moveToDown ? -1 : 1;
        return new Vector3(curPosition.x, curPosition.y + step, curPosition.z);
    }
}

