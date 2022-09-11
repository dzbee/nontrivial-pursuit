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
    private float timePerCharacter = 0.05f;

    private string HideTextFromIndex(string text, int index)
    {
        return $"{text.Substring(0, index)}<color=#00000000>{text.Substring(index)}</color>";
    }

    private void SetTriviaBackgroundSize(string trivia)
    {
        text.text = HideTextFromIndex(trivia, 0);
        text.ForceMeshUpdate();
        Vector2 textSize = text.GetRenderedValues(false);
        background.size = textSize + padding;
        transform.localPosition = startPos + new Vector3(background.size.x / 2 - startWidth, 0, 0);
    }

    private IEnumerator WriteTrivia(string trivia)
    {
        int characterIndex = 0;
        float timer = 0;
        while (true)
        {
            while (timer <= 0)
            {
                characterIndex++;
                if (characterIndex > trivia.Length)
                {
                    yield break;
                }
                text.text = HideTextFromIndex(trivia, characterIndex);
                timer += timePerCharacter;
            }
            yield return null;
            timer -= Time.deltaTime;
        }
    }

    public IEnumerator DisplayTrivia(string trivia)
    {
        SetTriviaBackgroundSize(trivia);
        yield return StartCoroutine(WriteTrivia(trivia));
    }

    private void Awake()
    {
        startPos = transform.localPosition;
        startWidth = background.size.x;
    }
}
