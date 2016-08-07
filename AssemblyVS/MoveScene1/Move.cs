using UnityEngine;

public class Move
{
    private bool _moveToLeft = false;
    public Vector3 GetNextPosition(Vector3 curPosition, Vector3 bounds)
    {
        if (curPosition.x >= bounds.x / 2 && !_moveToLeft)
        {
            _moveToLeft = true;
        }
        else if (curPosition.x <= - bounds.x / 2 && _moveToLeft)
        {
            _moveToLeft = false;
        }
        var step = _moveToLeft ? -1 : 1;
        return new Vector3(curPosition.x + step, curPosition.y, curPosition.z);
    }
}

