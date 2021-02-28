using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Panel : MonoBehaviour
{
    [SerializeField] private Text _whoWinText;
    [SerializeField] private Text _yourScore;
    [SerializeField] private Text _enemyScore;
    [SerializeField] private GameObject _resulPanel;
    [SerializeField] private GameObject _rulePanel;
    [SerializeField] private GameManager _gameManager;

    public void OpenResultPanel(string who , string pScore, string eScore)
    {
        _resulPanel.SetActive(true);
        _whoWinText.text = who;
        _yourScore.text = pScore;
        _enemyScore.text = eScore;
    }

    public void CloseResultPanel()
    {
        _resulPanel.SetActive(false);

    }

    public void Replay()
    {
        CloseResultPanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RulePane(bool mode)
    {
        if (mode)
        {
            _rulePanel.SetActive(true);
            _gameManager.IsCanHover = false;
        }
        else
        {
            _rulePanel.SetActive(false);
            _gameManager.IsCanHover = true;
        }
    }
}
