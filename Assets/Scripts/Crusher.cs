using System.Collections;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float restTime = 2.0f;
    [SerializeField] private float warnTime = 0.8f; // >= 0.5 s requirement
    [SerializeField] private float strikeDuration = 0.4f;

    [Header("Components")]
    [SerializeField] private GameObject crusherSlab;
    [SerializeField] private Collider strikeCollider;
    [SerializeField] private Renderer warningIndicator;

    [Header("Positions")]
    [SerializeField] private Vector3 upPosition = new Vector3(0f, 4f, 0f);
    [SerializeField] private Vector3 downPosition = new Vector3(0f, 0.5f, 0f);

    private Color normalColor = new Color(0.6f, 0.6f, 0.6f);
    private Color warnColor = new Color(1.0f, 0.4f, 0.0f);
    private Color strikeColor = new Color(1.0f, 0.0f, 0.0f);

    private void Start()
    {
        // Ensure hazard component deals 2 damage as required for crusher
        if (strikeCollider != null && strikeCollider.TryGetComponent(out Hazard hazard))
        {
            hazard.SetDamage(2);
        }

        StartCoroutine(CrusherCycle());
    }

    private IEnumerator CrusherCycle()
    {
        while (true)
        {
            // 1. Rest Phase: Collider disabled, slab raised, neutral color
            if (strikeCollider != null) strikeCollider.enabled = false;
            SetSlabPosition(upPosition);
            SetIndicatorColor(normalColor);

            yield return new WaitForSeconds(restTime);

            // 2. Warning Phase: Warn for warnTime (>= 0.5s), visual warning active
            SetIndicatorColor(warnColor);
            float warnTimer = 0f;
            while (warnTimer < warnTime)
            {
                warnTimer += Time.deltaTime;
                // Subtle shaking effect during warning
                if (crusherSlab != null)
                {
                    float shake = Mathf.Sin(warnTimer * 30f) * 0.05f;
                    crusherSlab.transform.localPosition = upPosition + new Vector3(shake, 0f, 0f);
                }
                yield return null;
            }

            // 3. Strike Phase: Collider enabled, slab slams down with damage 2
            SetIndicatorColor(strikeColor);
            SetSlabPosition(downPosition);
            if (strikeCollider != null) strikeCollider.enabled = true;

            yield return new WaitForSeconds(strikeDuration);

            // 4. Retract: Turn collider off, raise slab back up
            if (strikeCollider != null) strikeCollider.enabled = false;
            float retractTime = 0.5f;
            float elapsed = 0f;
            while (elapsed < retractTime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / retractTime;
                if (crusherSlab != null)
                {
                    crusherSlab.transform.localPosition = Vector3.Lerp(downPosition, upPosition, t);
                }
                yield return null;
            }
            SetSlabPosition(upPosition);
        }
    }

    private void SetSlabPosition(Vector3 localPos)
    {
        if (crusherSlab != null)
        {
            crusherSlab.transform.localPosition = localPos;
        }
    }

    private void SetIndicatorColor(Color color)
    {
        if (warningIndicator != null && warningIndicator.material != null)
        {
            warningIndicator.material.color = color;
        }
    }
}
