using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Apps.RealTime.SceneManagers
{ 
	[RequireComponent(typeof(BoxCollider))]
	public sealed class Portal : MonoBehaviour
	{
		// TODO configurable
		[SerializeField] string _nextScene;
		private GameObject _player;

		void Start()
		{
			_player = GameObject.FindWithTag("Player");
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other == _player)
			{
				StartCoroutine(TransitionAsync());
			}
		}

		IEnumerator TransitionAsync()
		{
			yield return SceneManager.LoadSceneAsync(_nextScene);
		}
	}
}