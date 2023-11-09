using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] [SkipFactory]
public abstract class BaseFactory<T> {
	[field: SerializeField] public int factorySize { private set; get; } = 4;
	
	protected List<T>  m_list;
	protected int  m_currentIndex = -1;
	protected T m_lastItem;

	public virtual T LastItem => m_lastItem;
	

	protected virtual void CreateFactory() {
		m_list = new();
		
		for (int i = 0; i < factorySize; i++) {
			GenerateItem();
		}
	}

	protected virtual void GenerateItem() {
		var newObject = CreateFactoryItem();

		m_list.Add(newObject);
	}

	public virtual bool IsFactoryInitialized() {
		var isInitialized = m_list != null;
		
		if (!isInitialized) Debug.LogError($"{nameof(T)} not initialized and still trying to use it!");

		return isInitialized;
	}

	/// <summary>
	/// returns next factory item in line, without removing it from internal list
	/// </summary>
	/// <returns>item of type T</returns>
	public virtual T Next() {
		if (!IsFactoryInitialized()) return default;
		
		if (m_list.Count == 0) GenerateItem();

		m_currentIndex = (m_currentIndex + 1) % m_list.Count;

		m_lastItem = m_list[m_currentIndex];

		return m_lastItem;
	}
	
	/// <summary>
	/// returns next factory item in line and removes it from internal list (typically should be followed by Return)
	/// </summary>
	/// <returns>item of type T</returns>
	public virtual T Take() {
		T nextItem = Next();

		m_list.Remove(nextItem);
		
		return nextItem;
	}
	
	/// <summary>
	/// Return item taken before or a totally new one to the internal list
	/// </summary>
	/// <param name="returnedItem"></param>
	public virtual void Return(T returnedItem) {
		m_list.Add(returnedItem);
	}

	/// <summary>
	/// Provide a way to create factory item automatically
	/// </summary>
	/// <returns></returns>
	protected abstract T CreateFactoryItem();
	
	/// <summary>
	/// Provide a way to Recycle/Destroy factory items fully
	/// </summary>
	public abstract void Recycle();
}