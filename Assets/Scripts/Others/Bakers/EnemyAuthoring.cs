using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
	public GameObject prefab;
	public int hp;
	public int stlength;
	class EnemyBaker : Baker<EnemyAuthoring>
	{
		public override void Bake(EnemyAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new EnemyData
			{
				hp = authoring.hp,
				stlength = authoring.stlength,
				prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic)
			});
		}
	}
}
