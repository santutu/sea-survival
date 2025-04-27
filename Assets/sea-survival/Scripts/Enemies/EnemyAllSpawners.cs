using System.Collections.Generic;
using Santutu.Core.Base.Runtime.Singletons;
using UnityEngine;

namespace sea_survival.Scripts.Enemies
{
    public class EnemyAllSpawners : SingletonMonoBehaviour<EnemyAllSpawners>
    {
        [SerializeField] public List<EnemySpawner> spawner = new();
    }
}