using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerEntityPosition : MonoBehaviour
{
	public Entity entity;

	public EntityManager entityManager;

	void Start()
	{
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

		UniTask.Delay(100);

		entity = entityManager.CreateEntityQuery(typeof(PlayerTag)).GetSingletonEntity();
	}

    void Update()
    {
		entityManager.SetComponentData(entity, new LocalTransform
		{
			Position = transform.position,
			Rotation = transform.rotation
		});
    }
}
