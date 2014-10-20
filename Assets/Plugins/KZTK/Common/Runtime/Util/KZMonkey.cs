using UnityEngine;
using System.Collections;

public class KZMonkey : MonoBehaviour
{
    public static Vector2 WorldToScreen(GameObject obj, Camera camera)
    {
        return camera.WorldToScreenPoint(obj.transform.position);
    }

    public Coroutine Co(IEnumerator e)
    {
        return StartCoroutine(e);
    }

    public IEnumerator WaitForScene(
        string sceneName, float limitInSecond = 10, bool enforceLimit = false)
    {
        yield return Co(UntilTrue(
            () => !Application.isLoadingLevel && Application.loadedLevelName.Equals(sceneName),
            limitInSecond,
            enforceLimit
        ));

        //yield return new WaitForSeconds(1);
    }

    public IEnumerator WaitForGameObject(
        string name, float limitInSecond = 10, bool enforceLimit = false)
    {
        yield return Co(UntilTrue(
            () => GameObject.Find(name) != null,
            limitInSecond,
            enforceLimit
        ));
    }

    public IEnumerator WaitForNotice(
        string notice, float limitInSecond = 10, bool enforceLimit = false)
    {
        bool received = false;

        KZNoticeCenter.Instance.AddObserver((n) =>
        {
            received = true;
        }, notice);

        yield return Co(UntilTrue(() => received, limitInSecond, enforceLimit));

    }

    public IEnumerator UntilTrue(
        System.Func<bool> predicate, float limitInSecond = 10, bool enforceLimit = false)
    {
        float elapsed = 0;
        while (!predicate())
        {
            yield return null;
            elapsed += Time.deltaTime;
            if (elapsed > limitInSecond)
            {
                Debug.LogWarning(
                     string.Format("waited for {0} seconds but got no result!",
                     elapsed));
                if (enforceLimit)
                {
                    Debug.LogWarning("quit waiting!");
                    break;
                }
            }
        }
    }
}
