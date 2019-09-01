using System.Collections;
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
    [SerializeField] public List<int> citizenSpawners;

    List<PoliceDemo> polices = new List<PoliceDemo>();
    List<EnemyDemo> enemies = new List<EnemyDemo>();
    List<CitizenDemo> citizens = new List<CitizenDemo>();

    OhiraPlusModel ohira = OhiraPlusModel.Instance();

    public void InstantiateCharacters() {
        DeployPolices();
        DeployEnemies();
        DeployCitizens();
    }

    /// <summary>
    /// 三种返回某种类型棋子的方法
    /// </summary>
    /// <returns></returns>
    public List<PoliceDemo> GetPolices() {
        return polices;
    }
    public List<EnemyDemo> GetEnemies() {
        return enemies;
    }
    public List<CitizenDemo> GetCitizens() {
        return citizens;
    }

    void DeployPolices() {
        foreach (int i in policeSpawners) {
            HexCellMesh cell = gridManager.Cells[i];
            PoliceDemo p = Instantiate<PoliceDemo>(policePrefab);
            p.transform.SetParent(this.transform);
            p.transform.position += cell.transform.position;

            cell.Occupant = p;
            cell.TransferToStatus(HexCellStatus.OCCUPIED_AND_UNSELECTED);

            polices.Add(p);
        }
    }
    void DeployEnemies() {
        foreach (int i in enemySpawners)
        {
            HexCellMesh cell = gridManager.Cells[i];
            EnemyDemo e = Instantiate<EnemyDemo>(enemyPrefab);
            e.transform.SetParent(this.transform);
            e.transform.position += cell.transform.position;

            cell.Occupant = e;
            cell.TransferToStatus(HexCellStatus.OCCUPIED_AND_UNSELECTED);

            enemies.Add(e);
        }
    }
    void DeployCitizens() {
        foreach (int i in citizenSpawners) {
            HexCellMesh cell = gridManager.Cells[i];
            CitizenDemo c = Instantiate<CitizenDemo>(citizenPrefab);
            c.transform.SetParent(this.transform);
            c.transform.position += cell.transform.position;

            c.setEventCallback(ohira.MovementCompletionCallback);   // 委托

            cell.Occupant = c;
            cell.TransferToStatus(HexCellStatus.OCCUPIED_AND_UNSELECTED);

            citizens.Add(c);
        }
    }

}
