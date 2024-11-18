using UnityEngine;
using SpriteFramework;

public class DataTableExample : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W)) {
            var lst = GameEntry.DataTable.DTRechargeShopDBModel.GetList();
            int count = lst.Count;
            for (int i = 0; i < count; i++) {
                GameEntry.Log("价格:{0}", lst[i].Price);
                GameEntry.Log("商品名称:{0}", lst[i].Name);
                GameEntry.Log("充值后获得虚拟货币:{0}", lst[i].Virtual);
                GameEntry.Log("充值商品图标:{0}", lst[i].Icon);
            }
        }
    }

}
