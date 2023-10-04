using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
	public GameObject enemyPrefab;
	class PlayerBaker : Baker<PlayerAuthoring>
	{
		public override void Bake(PlayerAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, typeof(PlayerTag));
			AddComponent(entity, new PlayerData
			{
				enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic)
			});
		}
	}
}
