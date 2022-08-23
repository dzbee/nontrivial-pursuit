using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriviaBubble : MonoBehaviour
{
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private TextMeshPro text;
    private Vector2 padding = new Vector2(1.25f, 1.25f);
    private Vector3 startPos;
    private float startWidth;

    public void DisplayTrivia(string trivia)
    {
        text.SetText(trivia);
        text.ForceMeshUpdate();
        Vector2 textSize = text.GetRenderedValues(false);
        background.size = textSize + padding;
        transform.localPosition = startPos + new Vector3(background.size.x / 2 - startWidth, 0, 0);
    }

    private void Awake()
    {
        startPos = transform.localPosition;
        startWidth = background.size.x;
    }
}
