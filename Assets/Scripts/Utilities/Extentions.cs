using UnityEngine;

public static class GameObjectExtentions {
	public static bool StrictTryGetComponent<T, M>(this M self, out T result) where T : Component where M : MonoBehaviour {
		if (self.gameObject.TryGetComponent<T>(out result) is false) {
			Debug.LogError($"{typeof(M)} requires a {typeof(T)} component on the same GameObject!");
			return false;
		} else return true;
	}


	public static T GetOrAddComponent<T, V>(this MonoBehaviour self) where T : Component where V : T
		=> self.gameObject.TryGetComponent(out T result) ? result : self.gameObject.AddComponent<V>();
	public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
		=> self.gameObject.TryGetComponent(out T result) ? result : self.gameObject.AddComponent<T>();

	
	public static T GetOrAddComponent<T, V>(this GameObject go) where T : Component where V : T
		=> go.TryGetComponent(out T result) ? result : go.AddComponent<V>();
}

