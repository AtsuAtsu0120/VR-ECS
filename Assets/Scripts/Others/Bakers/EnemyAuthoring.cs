using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemyAuthoring : MonoBehaviour
{
	class EnemyBaker : Baker<EnemyAuthoring>
	{
		public override void Bake(EnemyAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, typeof(EnemyTag));
		}
	}
}
