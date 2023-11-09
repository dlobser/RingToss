using UnityEngine;

[System.Serializable] [SkipFactory]
public class ComponentFactory<T> : BaseFactory<T> where T : Component {
	// [field: SerializeField] public bool instantiateInWorldSpace { private set; get; } = false;
	[field: SerializeField] public T prefab { private set; get; }

	private Transform parent;

	public void Create(Transform transform) {
		this.parent = transform;

		base.CreateFactory();
	}

	protected override T CreateFactoryItem() {
		return Object.Instantiate(prefab, parent, false);
	}

	public override void Recycle() {
		for (int i = m_list.Count - 1; i >= 0; i--) {
			Object.Destroy(m_list[i]);
		}

		m_list.Clear();
	}

	public T Take(Transform newParent) {
		var taken = base.Take();

		taken.transform.SetParent(newParent);

		return taken;
	}

	public override void Return(T returnedItem) {
		if (returnedItem == null) return;
		base.Return(returnedItem);
		returnedItem.transform.SetParent(parent);
	}
}