using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathIcon : MonoBehaviour
{
    public AnimationCurve curve;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(FloatUpAndFadeOut());
    }

    // Update is called once per frame
    IEnumerator FloatUpAndFadeOut()
    {
        float duration = 1;
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + Vector3.up * 1;
        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        while(t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(t/duration));
            sr.color = Color.Lerp(startColor, endColor, curve.Evaluate(t/duration));
            yield return null;
        }

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
