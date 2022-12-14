using System.Collections;
using UnityEngine;

namespace GoL.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvasGroup;
        Coroutine _currentActiveFade = null;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        public Coroutine Fade (float target, float time)
        {
            if (_currentActiveFade != null)
            {
                StopCoroutine(_currentActiveFade);
            }
            _currentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return _currentActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, target, Time.deltaTime / time);
                //Debug.Log($"alpha = {_canvasGroup.alpha}, target = {target}, time={time}, interval = {Time.deltaTime / time}"); MVP Brian saved me with this!
                yield return null;
            }
        }
    }
}