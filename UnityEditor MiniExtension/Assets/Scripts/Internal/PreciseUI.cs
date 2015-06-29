/*  
    Autor: Antonio Mateo Tomas (lPinchol)
    Date: 14/04/2015
    GitHub: https://github.com/lPinchol
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("EditorExt/Event/Precise UI"), ExecuteInEditMode]
public class PreciseUI : GraphicRaycaster 
{
    [Header("Precise UI test properties")]
    [Range(0, 1), Tooltip("Below that value of alpha components won't react to raycast.")]
    public float AlphaThreshold = .9f;
    [Tooltip("Include material tint color when checking alpha.")]
    public bool IncludeMaterialAlpha;
    [Tooltip("Will test alpha only on objects with Alpha Check component.")]
    public bool SelectiveMode;
    [Tooltip("Show warnings in the console when raycasting objects with a not-readable texture.")]
    public bool ShowTextureWarnings;

    private List<RaycastResult> toExclude = new List<RaycastResult>();

    protected override void OnEnable()
    {
        base.OnEnable();

        var badGuy = GetComponent<GraphicRaycaster>();
        if (badGuy && badGuy != this) DestroyImmediate(badGuy);
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        base.Raycast(eventData, resultAppendList);

        toExclude.Clear();

        foreach (var result in resultAppendList)
        {
            var objImage = result.gameObject.GetComponent<Image>();
            if (!objImage) continue;

            var objAlphaCheck = result.gameObject.GetComponent<PreciseUICheck>();
            if (SelectiveMode && !objAlphaCheck) continue;

            try
            {
                var objTrs = result.gameObject.transform as RectTransform;

                // evaluating pointer position relative to object local space
                Vector3 pointerGPos;
                if (eventCamera)
                {
                    var objPlane = new Plane(objTrs.forward, objTrs.position);
                    float distance;
                    var cameraRay = eventCamera.ScreenPointToRay(eventData.position);
                    objPlane.Raycast(cameraRay, out distance);
                    pointerGPos = cameraRay.GetPoint(distance);
                }
                else
                {
                    pointerGPos = eventData.position;
                    float rotationCorrection = (-objTrs.forward.x * (pointerGPos.x - objTrs.position.x) - objTrs.forward.y * (pointerGPos.y - objTrs.position.y)) / objTrs.forward.z;
                    pointerGPos += new Vector3(0, 0, objTrs.position.z + rotationCorrection);
                }
                Vector3 pointerLPos = objTrs.InverseTransformPoint(pointerGPos);

                // getting targeted pixel from object texture and evaluating its alpha
                var objTex = objImage.mainTexture as Texture2D;
                var texRect = objImage.sprite.textureRect;
                float texCorX = pointerLPos.x * (texRect.width / objTrs.sizeDelta.x) + texRect.width / 2;
                float texCorY = pointerLPos.y * (texRect.height / objTrs.sizeDelta.y) + texRect.height / 2;
                float alpha = objTex.GetPixel((int)(texCorX + texRect.x), (int)(texCorY + texRect.y)).a;

                // deciding if we need to exclude the object from results list
                if (objAlphaCheck)
                {
                    if (objAlphaCheck.IncludeMaterialAlpha) alpha *= objImage.color.a;
                    if (alpha < objAlphaCheck.AlphaThreshold) toExclude.Add(result);
                }
                else
                {
                    if (IncludeMaterialAlpha) alpha *= objImage.color.a;
                    if (alpha < AlphaThreshold) toExclude.Add(result);
                }
            }
            catch (UnityException e)
            {
                if (Application.isEditor && ShowTextureWarnings)
                    Debug.LogWarning(string.Format("Check for alpha failed: {0}", e.Message));
            };
        }

        resultAppendList.RemoveAll(r => toExclude.Contains(r));
    }
}
