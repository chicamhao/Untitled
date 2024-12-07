using System.Collections;
using UnityEngine;

namespace Apps.Runtime.SceneManagers
{ 
	[RequireComponent(typeof(BoxCollider))]
	public sealed class Portal : MonoBehaviour
	{
		// TODO configurable
		[SerializeField] string _nextScene;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(TransitionAsync());
			}
		}

		IEnumerator TransitionAsync()
		{
			yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_nextScene);
		}
	}
}