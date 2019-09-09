using System.Collections;
using System.Collections.Generic;


/*  2019/9/9：
 *  为避免 HexGridManager.SearchPath() 函数中的 frontier 使用 List数据结构造成的问题 —— 每次寻找优先搜索单位的时候都要遍历
 *  自定义了该类
 */
public class HexCellPriorityQueue
{
    List<HexCellMesh> list = new List<HexCellMesh>();
    int count = 0;
    public int Count {
        get { return count; }
    }

    int minimum = int.MaxValue;  //保存优先级最大的有效索引值

    /// <summary>
    /// 当一个 Cell 被添加进优先队列的时候，可简单地将其 Priority值 作为其保存的索引
    /// </summary>
    /// <param name="cell"></param>
    public void Enqueue(HexCellMesh cell) {
        count += 1;
        int priority = cell.SearchPriority;

        if (priority < minimum) {
            minimum = priority;
        }
        
        while (priority >= list.Count) {    //安全代码
            list.Add(null);
        }

        cell.NextWithSamePriority = list[priority];  // 创建了链表

        list[priority] = cell;  // List的中括号访问机制？
    }
    /// <summary>
    /// 出队时永远返回优先级更高的的待搜索项 —— 即在 list 中索引靠前的链表中的cell
    /// </summary>
    /// <returns></returns>
    public HexCellMesh Dequeue() {
        count -= 1;
        for (; minimum < list.Count; minimum++) {
            HexCellMesh cell = list[minimum];
            if (cell != null) {
                list[minimum] = cell.NextWithSamePriority;
                return cell;
            }
        }
        return null;
    }

    /// <summary>
    /// 改变某个 cell 的寻路优先级
    /// </summary>
    /// <param name="cell"> 待改变优先级的 cell </param>
    /// <param name="oldPriority"> 旧的优先级 </param>
    public void Change(HexCellMesh cell, int oldPriority) {
        HexCellMesh current = list[oldPriority];
        HexCellMesh next = current.NextWithSamePriority;
        if (current == cell)
        {
            list[oldPriority] = next;
        }
        else {
            while (next != cell) {
                current = next;
                next = current.NextWithSamePriority;
            }
            current.NextWithSamePriority = cell.NextWithSamePriority;       //典型的链表删除操作
            Enqueue(cell);
            count -= 1;
        }
    }
    public void Clear() {
        list.Clear();
        count = 0;
        minimum = int.MaxValue;
    }
}
