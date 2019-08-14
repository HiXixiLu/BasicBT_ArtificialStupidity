﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    public PoliceDemo policePrefab;
    public CitizenDemo citizenPrefab;
    public EnemyDemo enemyPrefab;

    public HexGridManager gridManager;

    [SerializeField] public List<int> policeSpawners;   // 暂保存cells里的索引值
    [SerializeField] public List<int> enemySpawners;

    List<PoliceDemo> polices = new List<PoliceDemo>();
    List<EnemyDemo> enemies = new List<EnemyDemo>();
    List<CitizenDemo> citizens = new List<CitizenDemo>();

    public void InstantiateCharacters() {
        DeployPolices();
        DeployEnemies();
    }

    void DeployPolices() {
        foreach (int i in policeSpawners) {
            HexCellMesh cell = gridManager.cells[i];
            PoliceDemo p = Instantiate<PoliceDemo>(policePrefab);
            p.transform.SetParent(this.transform);
            p.transform.position += cell.transform.position;

            cell.Occupant = p;

            polices.Add(p);
        }
    }
    void DeployEnemies() {
        foreach (int i in enemySpawners)
        {
            HexCellMesh cell = gridManager.cells[i];
            EnemyDemo e = Instantiate<EnemyDemo>(enemyPrefab);
            e.transform.SetParent(this.transform);
            e.transform.position += cell.transform.position;

            cell.Occupant = e;

            enemies.Add(e);
        }
    }
}