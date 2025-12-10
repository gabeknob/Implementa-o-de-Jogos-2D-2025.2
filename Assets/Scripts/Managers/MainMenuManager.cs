using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Nome da Cena do Jogo")]
    [Tooltip("Deve ser EXATAMENTE o nome do arquivo da sua cena de gameplay.")]
    public string gameSceneName = "GameScene";

    [Header("Painéis de UI")]
    public GameObject mainPanel;      // Onde ficam os botões Iniciar/Config/Sair
    public GameObject settingsPanel;  // Onde ficam as opções de volume, etc.

    void Start()
    {
        // Garante que o painel principal aparece e as configurações iniciam fechadas
        ShowMainPanel();
    }

    // --- FUNÇÕES DOS BOTÕES (Ligue no OnClick) ---

    public void OnPlayClicked()
    {
        // Carrega a cena do jogo (mudança de cena)
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsClicked()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnQuitClicked()
    {
        Debug.Log("O jogo foi fechado (funciona apenas na Build final).");
        Application.Quit();
    }

    // --- FUNÇÕES DO PAINEL DE CONFIGURAÇÕES ---

    public void OnBackFromSettings()
    {
        ShowMainPanel();
    }

    private void ShowMainPanel()
    {
        if(settingsPanel != null) settingsPanel.SetActive(false);
        if(mainPanel != null) mainPanel.SetActive(true);
    }
}