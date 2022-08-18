using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using GoL.Control;

namespace GoL.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int _sceneToLoad = -1;
        [SerializeField] Transform _spawnPoint;
        [SerializeField] DestinationIdentifier _destination;
        [SerializeField] float _fadeOutTime = 1f;
        [SerializeField] float _fadeInTime = 2f;
        [SerializeField] float _fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(_sceneToLoad < 0)
            {
                Debug.LogError("Scene to load is not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader _fader = FindObjectOfType<Fader>();
            SavingWrapper _savingWrapper = FindObjectOfType<SavingWrapper>();
            PlayerController _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            _playerController.enabled = false;

            yield return _fader.FadeOut(_fadeOutTime);

            _savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(_sceneToLoad);
            PlayerController _newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            _newPlayerController.enabled = false;

            _savingWrapper.Load();

            Portal _otherPortal = GetOtherPortal();
            UpdatePlayer(_otherPortal);

            _savingWrapper.Save();

            yield return new WaitForSeconds(_fadeWaitTime);
            _fader.FadeIn(_fadeInTime);

            _newPlayerController.enabled = true;
            Destroy(gameObject);
        }

        private void UpdatePlayer(Portal otherPortal)
        {
            GameObject _player = GameObject.FindWithTag("Player");
            _player.GetComponent<NavMeshAgent>().enabled = false;
            _player.GetComponent<NavMeshAgent>().Warp(otherPortal._spawnPoint.position);
            _player.transform.rotation = otherPortal._spawnPoint.rotation;
            _player.GetComponent<NavMeshAgent>().enabled = true;
        }

        private Portal GetOtherPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if (portal._destination != _destination) continue;
                return portal;
            }
            return null;
        }
    }
}