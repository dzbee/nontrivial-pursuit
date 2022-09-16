using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriviaBubble : MonoBehaviour
{
    [SerializeField] public SpriteRenderer background, pointer;
    [SerializeField] public TextMeshPro text;
    private Vector2 padding = new Vector2(0.5f, 0.5f);
    private float timePerCharacter = 0.05f;

    private string HideTextFromIndex(string text, int index)
    {
        return $"{text.Substring(0, index)}<color=#00000000>{text.Substring(index)}</color>";
    }

    public void SetTriviaBackgroundSize(string trivia)
    {
        text.text = HideTextFromIndex(trivia, 0);
        text.ForceMeshUpdate();
        Vector2 textSize = text.GetRenderedValues(false);
        Vector2 currentSize = background.size;
        background.size = textSize + padding;
        pointer.transform.Translate(new Vector2(0, (currentSize.y - background.size.y) / 2));
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
}
