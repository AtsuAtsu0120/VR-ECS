using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerDataAuthoring : MonoBehaviour
{
	public GameObject enemyPrefab;
	class PlayerDataBaker : Baker<PlayerDataAuthoring>
	{
		public override void Bake(PlayerDataAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new PlayerData
			{
				enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic)
			});
		}
	}
}
