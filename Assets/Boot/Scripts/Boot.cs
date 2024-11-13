using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ŀ����
/// </summary>
public class Boot : MonoBehaviour
{

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(BootStartUp());
    }

    /// <summary>
    /// ��Ŀ�������
    /// </summary>
    IEnumerator BootStartUp() {

        //����ȸ���
        yield return CheckHotUpdate();
        //end

        //��ܳ�ʼ��
        yield return InitFramework();
        //end

        //������Ϸ
        //end

        yield break;
    }

    /// <summary>
    /// ����ȸ���
    /// </summary>
    IEnumerator CheckHotUpdate() {
        yield break;
    }

    /// <summary>
    /// ��ܳ�ʼ��
    /// </summary>
    /// <returns></returns>
    IEnumerator InitFramework() {
        yield break;
    }

}
