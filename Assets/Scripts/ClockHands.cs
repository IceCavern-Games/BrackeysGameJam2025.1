using Reflex.Attributes;
using UnityEngine;

public class ClockHands : MonoBehaviour
{
    public Transform hours, minutes;

    [Inject] private readonly GameManager _gameManager;

    private const float _hoursToDegrees = 360f / 12f,
      _minutesToDegrees = 360f / 60f;

    private void Update()
    {
        if (_gameManager == null)
            return;

        int clockHours = (int)(9 + _gameManager.Clock.ElapsedTime / 60);
        int clockMinutes = (int)(_gameManager.Clock.ElapsedTime % 60);

        hours.localRotation = Quaternion.Euler(0f, 0f, (clockHours * _hoursToDegrees) + (clockMinutes * _minutesToDegrees / 12.0f));
        minutes.localRotation = Quaternion.Euler(0f, 0f, clockMinutes * _minutesToDegrees);
    }
}
