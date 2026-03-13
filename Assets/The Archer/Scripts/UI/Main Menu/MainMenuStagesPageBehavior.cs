using OctoberStudio.Easing;
using OctoberStudio.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OctoberStudio.UI
{
    public class MainMenuStagesPageBehavior : MainMenuPageBehavior
    {
        [Space]
        [SerializeField] protected StageDatabase stageDatabase;
        public StageDatabase StageDatabase => stageDatabase;

        [Header("Stage Data")]
        [SerializeField] protected MainMenuStageSelector stageSelector;

        [Header("Buttons")]
        [SerializeField] protected Button playButton;

        [Space]
        [SerializeField] protected Button settingsButton;
        [SerializeField] protected GameObject settingsGamepadIndicator;

        [Space]
        [SerializeField] protected RectTransform playButtonShineLine;
        [SerializeField] protected Sprite playButtonEnabledSprite;
        [SerializeField] protected Sprite playButtonDisabledSprite;

        public StageSave StageSave { get; protected set; }
        public ContinuePlayingSave ContinuePlayingSave { get; protected set; }

        protected override void Awake()
        {
            base.Awake();

            settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        }

        protected override void Start()
        {
            base.Start();

            playButton.onClick.AddListener(OnPlayerButtonClicked);

            StageSave = GameController.SaveManager.GetSave<StageSave>("Stage");
            ContinuePlayingSave = GameController.SaveManager.GetSave<ContinuePlayingSave>("Continue Playing");

            StageSave.onSelectedStageChanged += InitStage;
            InitStage(StageSave.SelectedStageId);

            if (!ContinuePlayingSave.HasUnfinishedStageData || ContinuePlayingSave.HP <= 0)
            {
                EventSystem.current.SetSelectedGameObject(playButton.gameObject);
                StageSave.SetSelectedStageId(StageSave.MaxReachedStageId);

                SubscribeToInputEvents();
            }
            else
            {
                GameController.MainMenuScreenBehavior.ContinuePlayingPopup.Show();
                GameController.MainMenuScreenBehavior.ContinuePlayingPopup.onPopupClosed += OnContinuePlayingPopupClosed;

                GameController.MainMenuScreenBehavior.DisableDock();
            }

            OnMoveCenterFinished();

            stageSelector.Init(this);
        }

        public virtual void SelectPlayButton()
        {
            EventSystem.current.SetSelectedGameObject(playButton.gameObject);
        }

        protected virtual void InitStage(int stageId)
        {
            var stage = stageDatabase.GetStageData(stageId);
            if (stage == null)
            {
                Debug.LogError($"Stage with ID {StageSave.SelectedStageId} not found in the database.");
                return;
            }

            if (StageSave.SelectedStageId > StageSave.MaxReachedStageId)
            {
                playButton.interactable = false;
                playButton.image.sprite = playButtonDisabledSprite;
            }
            else
            {
                playButton.interactable = true;
                playButton.image.sprite = playButtonEnabledSprite;

                SelectPlayButton();
            }
        }

        protected override void OnMoveCenterFinished()
        {
            base.OnMoveCenterFinished();

            if (!GameController.MainMenuScreenBehavior.ContinuePlayingPopup.IsOpen)
            {
                var linePosition = playButtonShineLine.anchoredPosition;
                var targetPosition = new Vector2(-linePosition.x, linePosition.y);

                playButtonShineLine.DoAnchorPosition(targetPosition, 0.5f).SetOnFinish(() => playButtonShineLine.anchoredPosition = linePosition);

                SubscribeToInputEvents();
                SelectPlayButton();
            }
        }

        public override void OnMoveFinished()
        {
            base.OnMoveFinished();

            UnsubscribeFromInputEvents();
        }

        public virtual void OnSettingsPopupClosed()
        {
            GameController.MainMenuScreenBehavior.SettingsPopup.onPopupClosed -= OnSettingsPopupClosed;
            SubscribeToInputEvents();

            GameController.MainMenuScreenBehavior.EnableDock();

            if(GameController.InputManager.ActiveInput == InputType.Gamepad)
            {
                EventSystem.current.SetSelectedGameObject(playButton.gameObject);
            }
        }

        public virtual void OnContinuePlayingPopupClosed()
        {
            GameController.MainMenuScreenBehavior.ContinuePlayingPopup.onPopupClosed -= OnContinuePlayingPopupClosed;
            SubscribeToInputEvents();

            GameController.MainMenuScreenBehavior.EnableDock();

            SelectPlayButton();
        }

        protected virtual void SubscribeToInputEvents()
        {
            UnsubscribeFromInputEvents();

            GameController.InputManager.onInputChanged += OnInputChanged;
            GameController.InputManager.InputAsset.UI.Settings.performed += OnSettingsInputClicked;

            stageSelector.SubscribeToInputEvents();
        }

        protected virtual void UnsubscribeFromInputEvents()
        {
            GameController.InputManager.onInputChanged -= OnInputChanged;
            GameController.InputManager.InputAsset.UI.Settings.performed -= OnSettingsInputClicked;

            stageSelector.UnsubscribeFromInputEvents();
        }

        protected virtual void OnPlayerButtonClicked()
        {
            GameController.AudioManager.PlayButtonClick();

            ContinuePlayingSave.Disable();
            GameController.TempGold.Clear();

            GameController.LoadStage();
        }

        protected virtual void OnSettingsInputClicked(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            OnSettingsButtonClicked();
        }

        protected virtual void OnSettingsButtonClicked()
        {
            GameController.AudioManager.PlayButtonClick();

            GameController.MainMenuScreenBehavior.SettingsPopup.Show();
            GameController.MainMenuScreenBehavior.SettingsPopup.onPopupClosed += OnSettingsPopupClosed;

            UnsubscribeFromInputEvents();

            GameController.MainMenuScreenBehavior.DisableDock();
        }

        protected virtual void OnInputChanged(InputType prevInputType, InputType inputType)
        {
            if (prevInputType == InputType.UIJoystick)
            {
                EventSystem.current.SetSelectedGameObject(playButton.gameObject);
            }

            settingsGamepadIndicator.SetActive(inputType == InputType.Gamepad);
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromInputEvents();

            StageSave.onSelectedStageChanged -= InitStage;
        }
    }
}