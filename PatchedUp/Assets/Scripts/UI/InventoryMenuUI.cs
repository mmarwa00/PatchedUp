using UnityEngine;
using StarterAssets;
using UnityEngine.UI;

public class InventoryMenuUI : MonoBehaviour {
    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private GameObject menuPanel;

    // Hierüber holen wir uns die Tasten direkt von der Spielfigur!
    [SerializeField] private StarterAssetsInputs _input;

    [Header("Tagebuch Seiten Setup")]
    [SerializeField] private GameObject[] bookPages;

    [Header("Blätter Navigation Buttons")]
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;

    private int currentPageIndex = 0;
    private bool isMenuOpen = false;
    public bool IsMenuOpen => isMenuOpen;

    void Start() {
        if (menuPanel != null) menuPanel.SetActive(false);

        if (nextPageButton != null) nextPageButton.onClick.AddListener(NextPage);
        if (prevPageButton != null) prevPageButton.onClick.AddListener(PreviousPage);
    }

    void Update() {
        if (_input != null && _input.openInventory) {
            _input.openInventory = false;
            ToggleMenu();
        }
    }

    private void ToggleMenu() {
        isMenuOpen = !isMenuOpen;

        if (menuPanel != null) {
            menuPanel.SetActive(isMenuOpen);
        }

        if (isMenuOpen) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (_input != null) _input.cursorInputForLook = false;

            currentPageIndex = 0;
            UpdateJournalDisplay();
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            if (_input != null) _input.cursorInputForLook = true;
        }
    }

    private void UpdateJournalDisplay() {
        for (int i = 0; i < bookPages.Length; i++) {
            bookPages[i].SetActive(false);
        }

        if (bookPages.Length > 0 && currentPageIndex < bookPages.Length) {
            bookPages[currentPageIndex].SetActive(true);

            InventorySlotSUI[] slotsOnPage = bookPages[currentPageIndex].GetComponentsInChildren<InventorySlotSUI>(true);

            for (int j = 0; j < slotsOnPage.Length; j++) {
                int itemIndexInBag = (currentPageIndex * slotsOnPage.Length) + j;

                if (itemIndexInBag < playerInventory.ItemsInBag.Count) {
                    slotsOnPage[j].SetItemSlot(playerInventory.ItemsInBag[itemIndexInBag]);
                }
                else {
                    slotsOnPage[j].ClearSlot();
                }
            }
        }

        if (prevPageButton != null) prevPageButton.gameObject.SetActive(currentPageIndex > 0);
        if (nextPageButton != null) nextPageButton.gameObject.SetActive(currentPageIndex < bookPages.Length - 1);
    }

    public void NextPage() {
        if (currentPageIndex < bookPages.Length - 1) {
            currentPageIndex++;
            UpdateJournalDisplay();
        }
    }

    public void PreviousPage() {
        if (currentPageIndex > 0) {
            currentPageIndex--;
            UpdateJournalDisplay();
        }
    }
}