using Unity.Entities;
using UnityEngine;

public struct PlayerTag : IComponentData
{

}
public struct PlayerData : IComponentData
{
	public int hp;
	public Entity enemyPrefab;
}
